using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Common
{
    public class AppConfiguration
    {
        public const string RabbitMq = "RabbitMQConfig";
        public const string Elastic = "ElasticConfig";
        public const string EmailConfirmationApi = "EmailConfirmationApi";
        public const string ActiveDirectoryApi = "ActiveDirectoryApi";
        public const string MojSystemApi = "MojSystemApi";

        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostToken { get; set; }
        public string ASyncHost { get; set; }
    }
}