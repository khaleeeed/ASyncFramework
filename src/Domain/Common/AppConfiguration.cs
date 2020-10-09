using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Common
{
    public class AppConfiguration
    {
        public const string RabbitMq = "RabbitMQConfig";
        public const string Elastic = "ElasticConfig";
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
