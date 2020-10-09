using ASyncFramework.Application.Common.Models;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetryFailuresLogic
{
    public interface IRetryFailuresMessageLogic
    {
        Task<Result> Retry(string messageJson);
    }
}