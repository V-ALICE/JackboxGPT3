﻿using Newtonsoft.Json;

namespace JackboxGPT3.Games.Quiplash2.Models
{
    public struct SendEntryRequest
    {
        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("questionId")]
        public int QuestionId { get; set; }
    }
}