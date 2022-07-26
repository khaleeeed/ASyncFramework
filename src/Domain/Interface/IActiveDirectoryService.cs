using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Model.Response;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface IActiveDirectoryService
    {
        Task<GenericServiceResponse<AsyncUser>> GetUserByUserName(string userName);
        Task<GenericServiceResponse<AsyncUser>> Login(string userName, string password);
    }
}