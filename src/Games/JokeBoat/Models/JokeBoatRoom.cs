using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.JokeBoat.Models
{
    public enum LobbyState
    {
        None,
        PostGame,
        CanStart,
        WaitingForMore,
        Countdown
    }

    public enum RoomState
    {
        None,
        Gameplay,
        Lobby,
        Logo,
        MakeSingleChoice,
        EnterSingleText
    }

    public struct JokeBoatRoom
    {
        //[JsonProperty("activeContentId")]
        //public string ActiveContentId { get; set; }

        //[JsonProperty("audience")]
        //public JRaw Audience { get; set; }

        [JsonProperty("classes")]
        public List<string> Classes { get; set; }

        //[JsonProperty("formattedActiveContentId")]
        //public string FormattedActiveContentId { get; set; }

        [JsonProperty("gameCanStart")]
        public bool GameCanStart { get; set; }

        [JsonProperty("gameFinished")]
        public bool GameFinished { get; set; }

        [JsonProperty("gameIsStarting")]
        public bool GameIsStarting { get; set; }

        [JsonProperty("isLocal")]
        public bool IsLocal { get; set; }

        [JsonProperty("lobbyState")]
        public LobbyState LobbyState { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("platformId")]
        public string PlatformId { get; set; }

        [JsonProperty("state")]
        public RoomState State { get; set; }

        [JsonProperty("textDescriptions")]
        public TextDescriptionsBlock TextDescriptions { get; set; }

        [JsonProperty("analytics")]
        public JRaw Analytics { get; set; }
    }

    public struct TextDescriptionsBlock
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}