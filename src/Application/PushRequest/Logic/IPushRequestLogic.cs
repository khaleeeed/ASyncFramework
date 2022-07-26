using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.PushRequestLogic;
using ASyncFramework.Domain.Model;
using ASyncFramework.Domain.Model.Response;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Logic
{
    public interface IPushRequestLogic
    {
        Task<Result> Push(Message message);
        Task<Result> Push(PushRequestCommand request);
    }
}