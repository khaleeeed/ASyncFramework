using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ASyncFramework.Domain.Model.Response
{
    public class TokenResponse
    {
        [DataMember(Name = "access_token")]
        public string  Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
