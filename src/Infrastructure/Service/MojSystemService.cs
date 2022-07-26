using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Service
{
    public class MojSystemService : IMojSystemService
    {
        private readonly ISystemRepository _SystemRepository;
        public MojSystemService(ISystemRepository systemRepository)
        {
            _SystemRepository = systemRepository;
        }

        public async Task<GenericServiceResponse<List<MOJSystemResponse>>> GetAllSystems()
        {
            List<MOJSystemResponse> mojSystems = new List<MOJSystemResponse>();
            var systems = await _SystemRepository.GetAll();
            foreach (var system in systems)
            {
                mojSystems.Add(new MOJSystemResponse { ArName = system.ArSystemName, IntgerationCode = system.SystemCode.ToString(), IsActive = system.IsActive });
            }
            return new GenericServiceResponse<List<MOJSystemResponse>> { IsSuccessful = true, Data = mojSystems, ResponseMessage = "OK", StatusCode = HttpStatusCode.OK };
        }

        public string GetSystemDetails(string systemCode)
        {
            var mojSystem = _SystemRepository.GetSystem(systemCode);
            return mojSystem?.ArSystemName;
        }
    }
}