// This file was generated with jb_api_gen.py
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JackboxGPT3.Games.JokeBoat.Models;

public struct JokeBoatRoom
{
	[JsonProperty("activeContentId")]
	public JRaw ActiveContentId { get; set; } // Always null in API data

	[JsonProperty("audience")]
	public AudienceBlock? Audience { get; set; }

	[JsonProperty("classes")]
	public List<string> Classes { get; set; }

	[JsonProperty("formattedActiveContentId")]
	public JRaw FormattedActiveContentId { get; set; } // Always null in API data

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
	public State State { get; set; }

	[JsonProperty("textDescriptions")]
	public List<TextDescriptionsEntry> TextDescriptions { get; set; }

	[JsonProperty("analytics")]
	public List<AnalyticsEntry> Analytics { get; set; }
}

public struct AudienceBlock
{
	[JsonProperty("state")]
	public State State { get; set; }
}

public struct TextDescriptionsEntry
{
	[JsonProperty("category")]
	public string Category { get; set; }

	[JsonProperty("id")]
	public int Id { get; set; }

	[JsonProperty("text")]
	public string Text { get; set; }
}

public struct AnalyticsEntry
{
	[JsonProperty("appid")]
	public string Appid { get; set; }

	[JsonProperty("appname")]
	public string Appname { get; set; }

	[JsonProperty("appversion")]
	public string Appversion { get; set; }

	[JsonProperty("screen")]
	public string Screen { get; set; }

	[JsonProperty("action")]
	public string Action { get; set; }

	[JsonProperty("category")]
	public string Category { get; set; }

	[JsonProperty("value")]
	public int Value { get; set; }
}
