using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.JokeBoat.Models
{
    public enum ChoiceType
    {
        None,
        ChooseSetup,
        ChooseTopic,
        ChooseAuthorReady,
        ChooseJoke,
        ChoosePunchUpJoke,
        Skip
    }

    public struct JokeBoatPlayer
    {
        [JsonProperty("choiceId")]
        public string ChoiceId { get; set; }

        [JsonProperty("choiceType")]
        public ChoiceType ChoiceType { get; set; }

        [JsonProperty("choices")]
        public List<ChoiceBlock> Choices { get; set; }

        [JsonProperty("classes")]
        public JRaw Classes { get; set; }

        [JsonProperty("doneText")]
        public JRaw DoneText { get; set; } // Can be a string or a json object (or null)

        [JsonProperty("playerInfo")]
        public JRaw PlayerInfo { get; set; }

        [JsonProperty("prompt")]
        public HtmlBlock Prompt { get; set; }

        [JsonProperty("state")]
        public RoomState State { get; set; }

        [JsonProperty("playerCanCensor")]
        public bool PlayerCanCensor { get; set; }

        [JsonProperty("playerCanDoUGC")]
        public bool PlayerCanDoUgc { get; set; }

        [JsonProperty("playerCanReport")]
        public bool PlayerCanReport { get; set; }

        [JsonProperty("playerCanStartGame")]
        public bool PlayerCanStartGame { get; set; }

        [JsonProperty("playerCanViewAuthor")]
        public bool PlayerCanViewAuthor { get; set; }

        [JsonProperty("playerIsVIP")]
        public bool PlayerIsVip { get; set; }

        [JsonProperty("counter")]
        public bool Counter { get; set; }

        [JsonProperty("entry")]
        public bool Entry { get; set; }

        [JsonProperty("entryId")]
        public string EntryId { get; set; }

        //[JsonProperty("error")]
        //public string Error { get; set; }

        [JsonProperty("inputType")]
        public string InputType { get; set; }

        [JsonProperty("maxLength")]
        public int MaxLength { get; set; }

        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        [JsonProperty("actions")]
        public List<ActionBlock> Actions { get; set; }

        [JsonProperty("autoSubmit")]
        public bool AutoSubmit { get; set; }

        [JsonProperty("autocapitalize")]
        public bool Autocapitalize { get; set; }

        [JsonProperty("message")]
        public HtmlBlock Message { get; set; }

        [JsonProperty("announcePrompt")]
        public bool AnnouncePrompt { get; set; }

        [JsonProperty("chosen")]
        public int Chosen { get; set; }

        [JsonProperty("canDoUGC")]
        public bool CanDoUgc { get; set; }

        [JsonProperty("history")]
        public JRaw History { get; set; }

        //[JsonProperty("lastUGCResult")]
        //public JRaw LastUgcResult { get; set; }
    }

    public struct HtmlBlock
    {
        [JsonProperty("html")]
        public string Html { get; set; }
    }

    public struct ChoiceBlock
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

    public struct ActionBlock
    {
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }
    }
}