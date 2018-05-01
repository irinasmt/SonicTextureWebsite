using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication2.Models.MessageApi
{
    public class SkillMessagingApiRequest
    {
        [JsonProperty("grant_type")] public string GrantType => "client_credentials";

        [JsonProperty("client_id")]
        public string ClientId => "amzn1.application-oa2-client.572f889a4f314c558f95535f66a3a3f5";

        [JsonProperty("client_secret")] public string ClientSecret => "46b2fc6fb81271a069fbf8fd359d1899fe235c8ba6e0911ddfb880654edd07a6";


        [JsonProperty("scope")] public string Scope => "alexa:skill_messaging";
    }
}