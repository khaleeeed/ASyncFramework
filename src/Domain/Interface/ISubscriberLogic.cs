using ASyncFramework.Domain.Model;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface ISubscriberLogic
    {
        Task Subscribe(Message message);
        Task InternalExceptionRetry(Message message);
    }
}