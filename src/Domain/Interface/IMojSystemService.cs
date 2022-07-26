using ASyncFramework.Domain.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface IMojSystemService
    {
        Task<GenericServiceResponse<List<MOJSystemResponse>>> GetAllSystems();
        string GetSystemDetails(string systemCode);
    }
}