// This file was generated with jb_api_gen.py
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.JokeBoat.Models;

public enum State
{
	EnterSingleText,
	Gameplay,
	Lobby,
	Logo,
	MakeSingleChoice
}

public enum LobbyState
{
	CanStart,
	Countdown,
	PostGame,
	WaitingForMore
}

public enum ChoiceType
{
	ChooseAuthorReady,
	ChooseJoke,
	ChoosePunchUpJoke,
	ChooseSetup,
	ChooseTopic,
	Skip
}

public struct JokeBoatPlayer
{
	[JsonProperty("choiceId")]
	public string ChoiceId { get; set; }

	[JsonProperty("choices")]
	public List<ChoicesEntry> Choices { get; set; }

	[JsonProperty("classes")]
	public List<string> Classes { get; set; }

	[JsonProperty("doneText")]
	public JRaw DoneText { get; set; } // Can be multiple types: Object or string (or null)

	[JsonProperty("playerInfo")]
	public PlayerInfoBlock PlayerInfo { get; set; }

	[JsonProperty("prompt")]
	public PromptBlock Prompt { get; set; }

	[JsonProperty("state")]
	public State State { get; set; }

	[JsonProperty("playerCanCensor")]
	public bool PlayerCanCensor { get; set; }

	[JsonProperty("playerCanDoUGC")]
	public bool PlayerCanDoUGC { get; set; }

	[JsonProperty("playerCanReport")]
	public bool PlayerCanReport { get; set; }

	[JsonProperty("playerCanStartGame")]
	public bool PlayerCanStartGame { get; set; }

	[JsonProperty("playerCanViewAuthor")]
	public bool PlayerCanViewAuthor { get; set; }

	[JsonProperty("playerIsVIP")]
	public bool PlayerIsVIP { get; set; }

	[JsonProperty("counter")]
	public bool Counter { get; set; }

	[JsonProperty("entry")]
	public bool Entry { get; set; }

	[JsonProperty("entryId")]
	public string EntryId { get; set; }

	[JsonProperty("error")]
	public string Error { get; set; }

	[JsonProperty("inputType")]
	public string InputType { get; set; }

	[JsonProperty("maxLength")]
	public int MaxLength { get; set; }

	[JsonProperty("placeholder")]
	public string Placeholder { get; set; }

	[JsonProperty("choiceType")]
	public ChoiceType ChoiceType { get; set; }

	[JsonProperty("actions")]
	public List<ActionsEntry> Actions { get; set; }

	[JsonProperty("autoSubmit")]
	public bool AutoSubmit { get; set; }

	[JsonProperty("autocapitalize")]
	public bool Autocapitalize { get; set; }

	[JsonProperty("message")]
	public MessageBlock Message { get; set; }

	[JsonProperty("announcePrompt")]
	public bool AnnouncePrompt { get; set; }

	[JsonProperty("chosen")]
	public int Chosen { get; set; }

	[JsonProperty("canDoUGC")]
	public bool CanDoUGC { get; set; }

	[JsonProperty("history")]
	public JRaw History { get; set; } // Always empty list in API data

	[JsonProperty("lastUGCResult")]
	public JRaw LastUGCResult { get; set; } // Always null in API data
}

public struct ChoicesEntry
{
	[JsonProperty("html")]
	public string Html { get; set; }

	[JsonProperty("text")]
	public string Text { get; set; }

	[JsonProperty("help")]
	public string Help { get; set; }

	[JsonProperty("disabled")]
	public bool Disabled { get; set; }
}

public struct PlayerInfoBlock
{
	[JsonProperty("avatar")]
	public string Avatar { get; set; }

	[JsonProperty("index")]
	public int Index { get; set; }

	[JsonProperty("playerIndex")]
	public int PlayerIndex { get; set; }

	[JsonProperty("username")]
	public string Username { get; set; }
}

public struct PromptBlock
{
	[JsonProperty("html")]
	public string Html { get; set; }
}

public struct ActionsEntry
{
	[JsonProperty("action")]
	public string Action { get; set; }

	[JsonProperty("html")]
	public string Html { get; set; }
}

public struct MessageBlock
{
	[JsonProperty("html")]
	public string Html { get; set; }
}