using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Infrastructure.Service.model.Response
{
    public class AccessTokenModel
    {
        [JsonProperty(PropertyName = "access_token")]
        public string Token { get; set; }
    }
}
