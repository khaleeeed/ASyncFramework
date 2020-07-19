using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ASyncFramework.Domain.Model
{
    public class AuthModel
    {
        [JsonPropertyName("access_Token")]
        public string token { get; set; }
    }
}
