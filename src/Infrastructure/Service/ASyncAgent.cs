using ASyncFramework.Domain.Interface;
using Elastic.Apm;
using Elastic.Apm.Api;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Service
{
    public class ASyncAgent : IASyncAgent
    {
        public Task SetResultToCurrentTransaction(bool? isSuccess, HttpStatusCode? StatusCode)
        {            
            Agent.Tracer.CurrentTransaction.Result = StatusCode ==null ? "Unknown":Convert.ToInt32(StatusCode).ToString();
            Agent.Tracer.CurrentTransaction.Outcome = isSuccess==null || isSuccess==false ? Outcome.Failure : Outcome.Success;
            return Task.CompletedTask;
        }
    }
}