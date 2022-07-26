using ASyncFramework.Domain.Model.Response;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface ISendEmailService
    {
        Task<GenericServiceResponse<object>> SendConfirmationEmail(string userName, string systemName, string url);
    }
}