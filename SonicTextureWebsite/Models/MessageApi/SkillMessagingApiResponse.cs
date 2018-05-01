using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication2.Models.MessageApi
{
    public class SkillMessagingApiResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("scope")] public string Scope { get; set; }

        [JsonProperty("token_type")] public string TokenType { get; set; }
    }
}