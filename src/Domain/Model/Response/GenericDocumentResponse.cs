using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Model.Response
{
    public class GenericDocumentResponse<T>
    {
        public long recordsTotal { get; set; }
        public long recordsFiltered { get; set; }
        public IEnumerable<T> data { get; set; }   
        
        public string Message { get; set; }
    }
}



