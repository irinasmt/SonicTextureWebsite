using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication2.Models.MessageApi
{
    public class GetTokenRequest
    {
        [JsonProperty("message")]
        public MessageAttribute Message { get; set; }

        [JsonObject("message")]
        public class MessageAttribute
        {
            [JsonProperty("data")]
            public dynamic Data { get; set; }
        }
    }
}