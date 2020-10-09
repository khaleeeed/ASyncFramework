using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Model.Response
{
    public class GenericDocumentResponse<T>
    {
        public IEnumerable<T> Document { get; set; }

        public long Total { get; set; }

        public string Message { get; set; }
    }
}
