using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace ASyncFramework.Domain.Interface
{
    public interface IASyncFrameworkInfrastructureRepository
    {
        Task<object> GetMessageStatus(string referenceNumber,int from);
        Task<object> GetCallBackResponse(string referenceNumber, int from);
        Task<object> GetTargetResponse(string referenceNumber, int from);
        Task<object> GetMessageFromCallBackUrl(string CallBackUrl, int from);
        Task<object> GetMessageByContentBodyForAdmin(string fieldName, string fieldValue, int from);
        Task<object> GetMessageByContentBodyForSystem(string fieldName, string fieldValue, int from, string systemCode);
    }
}