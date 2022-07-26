using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ASyncFramework.Domain.Model.Response
{
    public class GenericServiceResponse<T>
    {
        public T Data { get; set; }
        public bool IsSuccessful { get; set; }
        public string ResponseMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
