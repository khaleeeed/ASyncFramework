using ASyncFramework.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Model.Request
{
    public class BaseRequest
    {
        
        public string Url { get; set; }
        public ServiceType ServiceType { get; set; }

        /// <summary>
        /// defualt rest application/json
        /// defualt soap text/xml 
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// required if service type soap you can get from wsdl 
        /// </summary>
        public string SoapAction { get; set; }

    }
}
