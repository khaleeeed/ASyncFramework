using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface IASyncAgent
    {
        Task SetResultToCurrentTransaction(bool? isSuccess, HttpStatusCode? StatusCode);
    }
}
