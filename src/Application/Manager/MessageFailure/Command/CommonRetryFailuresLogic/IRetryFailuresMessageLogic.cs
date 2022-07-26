using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Model.Response;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetryFailuresLogic
{
    public interface IRetryFailuresMessageLogic
    {
        Task<Result> Retry(string messageJson);
    }
}