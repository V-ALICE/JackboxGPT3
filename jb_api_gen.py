import builtins
import json
import os
import pathlib
import sys
from dataclasses import dataclass, field
from io import TextIOWrapper
from typing import Any

# There's probably an existing tool that can do this sort of thing (as in JSON -> C#)
# but I like reinventing the wheel (aka I like writing things)


def _capitalize(s: str) -> str:
    if len(s) == 0:
        return s
    return s[0].upper() + s[1:]


class FieldData:

    def __init__(self, name: str) -> None:
        self._name = name
        self._types: set[type] = set()
        self._subdata: dict[str, "FieldData"] = {}
        self._can_be_none: bool = False

    def update(self, val: Any) -> None:
        if val is None:
            self._can_be_none = True
            return

        next_type = type(val)
        self._types.add(next_type)

        if next_type is list and len(val) > 0:
            if "entry" not in self._subdata:
                self._subdata["entry"] = self.__class__("entry")
            self._subdata["entry"].update(val[0])
        elif next_type is dict and len(val) > 0:
            for key, value in val.items():
                if key not in self._subdata:
                    self._subdata[key] = self.__class__(key)
                self._subdata[key].update(value)

    def print(self, indent_level: int = 0) -> None:
        header = f'    {"".ljust(indent_level)}{self._name} ->'
        if len(self._subdata) == 0:  # basic type(s) only
            field_str = ""
            for entry in self._types:
                field_str += f"{entry.__name__} "
            if self._can_be_none:
                field_str += "NULL"
            print(f"{header} {field_str}")
        elif len(self._types) == 1 and next(iter(self._types)) == list:  # handle list manually
            inner_type = next(iter(self._subdata["entry"]._types))
            list_typing = inner_type.__name__ if "entry" in self._subdata else "unknown"
            if len(self._subdata["entry"]._types) > 1:
                print('{header} WARNING: Unhandled type "list of complex type"')
            elif inner_type is list:
                print('{header} WARNING: Unhandled type "list of lists"')
            elif inner_type is dict:
                print(f'{header} list{" (can be NULL)" if self._can_be_none else ""}:')
                self._subdata["entry"].print(indent_level + 4)
            else:
                print(f'{header} list[{list_typing}]{" NULL" if self._can_be_none else ""}')
        elif len(self._types) == 1 and next(iter(self._types)) == dict:  # recurse
            print(f'{header} object{" (can be NULL)" if self._can_be_none else ""}:')
            for field in self._subdata.values():
                field.print(indent_level + 4)
        else:  # complicated
            field_str = ""
            for entry in self._types:
                field_str += f"{entry.__name__} "
            if self._can_be_none:
                field_str += "NULL"
            print(f"{header} Complex Type ({field_str})")

    def print_cs(self, file: TextIOWrapper, enum_names: set[str]) -> tuple[str, dict[str, "FieldData"]]:
        # Assumed to already be inside the class
        def _python_type_to_cs_type(t: type) -> str:
            match t:
                case builtins.str:
                    return "string"
                case builtins.int:
                    return "int"
                case builtins.float:
                    return "double"
                case builtins.bool:
                    return "bool"
                case _:
                    return "UNHANDLED"

        # Some fields come in as ints and floats at different times, treat such fields as floats
        if len(self._types) > 1 and all(t == int or t == float for t in self._types):
            self._types.clear()
            self._types.add(float)

        followup_name = None
        followup_obj: "FieldData" = None

        type_name = "UNKNOWN"
        comment = ""
        if self._name in enum_names:  # Enum type
            type_name = _capitalize(self._name)
        elif len(self._types) == 0:  # Type that is always null, just comment it out
            type_name = "JRaw"
            comment = " // Always null in API data"
        elif len(self._types) == 1:  # Only one type, easy
            if len(self._subdata) == 0:
                this_type = next(iter(self._types))
                if this_type == list or this_type == dict:
                    type_name = "JRaw"
                    comment = " // Always empty list in API data"
                else:
                    type_name = _python_type_to_cs_type(this_type)
            elif next(iter(self._types)) == list:
                inner_type = next(iter(self._subdata["entry"]._types))
                if len(self._types) == 0:
                    type_name = "JRaw"
                    comment = " // Always empty in API data"
                elif len(self._subdata["entry"]._types) > 1:
                    type_name = "JRaw"
                    comment = ' // WARNING: Unhandled type "list of complex type"'
                elif inner_type is list:
                    type_name = "JRaw"
                    comment = ' // WARNING: Unhandled type "list of lists"'
                elif inner_type is dict:
                    followup_name = f"{_capitalize(self._name)}Entry"
                    followup_obj = self._subdata["entry"]._subdata
                    type_name = f"List<{followup_name}>"
                else:
                    type_name = f'List<{_python_type_to_cs_type(next(iter(self._subdata["entry"]._types)))}>'
            elif next(iter(self._types)) == dict:
                followup_name = f"{_capitalize(self._name)}Block"
                followup_obj = self._subdata
                type_name = followup_name
        else:  # More than one type, complicated
            type_name = "JRaw"
            comment = " // Can be multiple types: "
            for entry in self._types:
                if entry == list:
                    comment += f"List or "
                elif entry == dict:
                    comment += f"Object or "
                else:
                    comment += f"{_python_type_to_cs_type(entry)} or "
            comment = comment[:-4]
            if self._can_be_none:
                comment += " (or null)"

        if type_name != "JRaw" and self._can_be_none:
            type_name += "?"

        file.write(f'\n\t[JsonProperty("{self._name}")]\n')
        file.write(f"\tpublic {type_name} {_capitalize(self._name)} {{ get; set; }}{comment}\n")
        return followup_name, followup_obj


@dataclass
class DataSet:
    send_data: list[set[str]] = field(default_factory=list)
    room_data: dict[str, FieldData] = field(default_factory=dict)
    player_data: dict[str, FieldData] = field(default_factory=dict)
    enum_data: dict[str, set[str]] = field(default_factory=dict)


def _get_receive_data(msg_data: dict, data: dict[str, FieldData]) -> None:
    for field_name, field_val in msg_data.items():
        if field_name not in data:
            data[field_name] = FieldData(field_name)
        data[field_name].update(field_val)


def _get_send_data(msg_data: dict, data: list[set[str]], special_keys: list[str]) -> None:
    new_set: set[str] = set()
    for field_name, field_val in msg_data.items():
        if field_name in special_keys:
            new_set.add(f'{field_name}#"{field_val}"')
        else:
            new_set.add(f"{field_name}#{type(field_val).__name__}")

    if all(new_set != existing_set for existing_set in data):
        data.append(new_set)


# This is a slow way to do this but whatever
# For the record, this is here so that json can be extracted while ignoring
# other logging artifacts (things like "[loader] load success" or "D_0e_jtX.js:12:238")
def _transform_to_json(lines: list[str], required: str = None) -> list[dict]:
    bracket_index: int = 0
    cur_str: str = ""

    def _sort_char(c: str) -> bool:
        nonlocal bracket_index
        nonlocal cur_str

        if c == "{":
            bracket_index += 1
            cur_str += c
        elif c == "}" and bracket_index > 0:
            bracket_index -= 1
            cur_str += c
            return bracket_index == 0
        elif bracket_index > 0:
            cur_str += c
        return False

    json_msgs: list[dict] = []
    for line in lines:
        for c in line:
            if _sort_char(c):
                try:
                    msg = json.loads(cur_str)
                    if required is None or required in msg:
                        json_msgs.append(msg)
                except ValueError:
                    pass
                finally:
                    cur_str = ""

    return json_msgs


def _output_template_files(game_name: str, src_folder_path: str, using_bc: bool) -> None:
    client_class = "BcSerializedClient" if using_bc else "PlayerSerializedClient"

    # Folder will always exist since it was created earlier
    client = pathlib.Path(f"{src_folder_path}/Games/{game_name}/{game_name}Client.cs")
    if not client.exists():
        with open(client, "w") as file:
            file.write(
                f"""using JackboxGPT3.Games.Common;
using JackboxGPT3.Games.{game_name}.Models;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Games.{game_name};

public class {game_name}Client : {client_class}<{game_name}Room, {game_name}Player>
{{
    public {game_name}Client(IConfigurationProvider configuration, ILogger logger, int instance)
        : base(configuration, logger, instance)
    {{
    }}
}}"""
            )
    else:
        print(f"Skipping Client template creation as {game_name}Client.cs already exists")

    # If this folder doesn't exist something is wrong anyway
    engine = pathlib.Path(f"{src_folder_path}/Engines/{game_name}Engine.cs")
    if not engine.exists():
        with open(engine, "w") as file:
            file.write(
                f"""using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.{game_name};
using JackboxGPT3.Games.{game_name}.Models;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Engines;

public class {game_name}Engine : BaseJackboxEngine<{game_name}Client>
{{
    protected override string Tag => "TODO";

    public {game_name}Engine(ICompletionService completionService, ILogger logger, {game_name}Client client, int instance)
        : base(completionService, logger, client, instance)
    {{
        JackboxClient.OnSelfUpdate += OnSelfUpdate;
        JackboxClient.OnRoomUpdate += OnRoomUpdate;
        JackboxClient.Connect();
    }}

    private void OnSelfUpdate(object sender, Revision<{game_name}Player> revision)
    {{
    }}

    private void OnRoomUpdate(object sender, Revision<{game_name}Room> revision)
    {{
    }}
}}"""
            )
    else:
        print(f"Skipping Engine template creation as {game_name}Engine.cs already exists")


def _output_as_cs(data: DataSet, game_name: str, src_folder_path: str, using_bc: bool) -> None:
    header = f"""// This file was generated with jb_api_gen.py
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.{game_name}.Models;
"""

    def _handle_enum(file: TextIOWrapper, enum_data: dict[str, set[str]]) -> None:
        for name, vals in enum_data.items():
            if len(vals) == 0:
                continue

            file.write(f"\npublic enum {_capitalize(name)}\n{{\n")
            started = False
            for val in sorted(vals):
                if started:
                    file.write(f",\n\t{val}")
                else:
                    started = True
                    file.write(f"\t{val}")
            file.write("\n}\n")

    def _handle_basic(file: TextIOWrapper, class_name: str, struct_data: dict[str, FieldData], enums: set[str]) -> None:
        followups: list[tuple[str, dict[str, FieldData]]] = []

        file.write(f"\npublic struct {class_name}\n{{")
        for field in struct_data.values():
            re = field.print_cs(file, enums)
            if re[1] is not None:
                followups.append(re)
        file.write("}\n")

        for name, field in followups:
            _handle_basic(file, name, field, enums)

    model_folder = pathlib.Path(f"{src_folder_path}/Games/{game_name}/Models")
    model_folder.mkdir(parents=True, exist_ok=True)

    with open(f"{model_folder}/{game_name}Player.cs", "w") as file:
        file.write(header)
        _handle_enum(file, data.enum_data)  # TODO: put enums in other file if they're not used in this one
        _handle_basic(file, f"{game_name}Player", data.player_data, data.enum_data.keys())

    with open(f"{model_folder}/{game_name}Room.cs", "w") as file:
        file.write(header)
        _handle_basic(file, f"{game_name}Room", data.room_data, data.enum_data.keys())

    _output_template_files(game_name, src_folder_path, using_bc)


def _basic_print(data: DataSet, verbose: bool) -> None:
    if verbose:
        print(f"\nRoom Fields:")
        for field in data.room_data.values():
            field.print()

        print("\nPlayer Fields:")
        for field in data.player_data.values():
            field.print()

        print("\nEnums:")
        for enum, val_set in data.enum_data.items():
            print(f"    {enum} values: {val_set}")

    print("\nSent Messages:")
    for entry in data.send_data:
        msg_str = ""
        for field in sorted(entry):
            if len(msg_str) > 0:
                msg_str += ", "
            split = field.split("#")
            msg_str += f"{split[0]} ({split[1]})"
        print(f"    {{ {msg_str} }}")


if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Usage: jb_api_gen.py <game_name> <input_folder> [enum_name...]")
        exit(1)

    src_dir = pathlib.Path(__file__).resolve().parent / "src"
    if not (src_dir / "Engines").is_dir():
        print("ERROR: this script is intended to be run inside the JackboxGPT3 repo folder")
        exit(1)

    lines: list[str] = []
    for path in [f for f in os.listdir(sys.argv[2]) if f.endswith(".txt") or f.endswith(".json")]:
        with open(f"{sys.argv[2]}/{path}", encoding="utf-8") as file:
            lines.extend(file.readlines())

    data = DataSet()
    if len(sys.argv) > 4:
        for enum in sys.argv[3:]:
            data.enum_data[enum] = set()

    using_bc: bool = None
    action_keys = ["action"]
    json_msgs: list[dict] = _transform_to_json(lines, "opcode")
    for msg in json_msgs:
        match msg["opcode"]:
            case "object":
                for key in data.enum_data.keys():
                    if key in msg["result"]["val"]:
                        data.enum_data[key].add(msg["result"]["val"][key])

                if using_bc is None:
                    using_bc = msg["result"]["key"].startswith("bc:")
                if msg["result"]["key"] == "bc:room" or msg["result"]["key"] == "room":
                    _get_receive_data(msg["result"]["val"], data.room_data)
                elif msg["result"]["key"].startswith("bc:customer:") or msg["result"]["key"].startswith("player:"):
                    _get_receive_data(msg["result"]["val"], data.player_data)
                else:
                    print(f'WARNING: Unhandled object key {msg["result"]["key"]}')
            case "client/send":
                _get_send_data(msg["params"]["body"], data.send_data, action_keys)
            case "client/welcome":
                pass
            case "room/lock":
                pass
            case "room/exit":
                pass
            case "ok":
                pass
            case _:
                print(f'WARNING: Unhandled opcode key {msg["opcode"]}')

    _output_as_cs(data, sys.argv[1], src_dir, using_bc)
    _basic_print(data, False)
