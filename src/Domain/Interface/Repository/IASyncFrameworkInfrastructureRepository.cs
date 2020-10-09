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
        (bool IsSendBefore, string ReferenceNumber) CheckIfRequestSendBefore(string hash);
    }
}