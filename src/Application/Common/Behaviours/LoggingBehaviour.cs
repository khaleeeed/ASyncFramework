using ASyncFramework.Application.Common.Hashing;
using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Application.PushRequestLogic;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Common.Behaviours
{
    /// <summary>
    ///  class will log every request come from Mediatr 
    /// </summary>
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly IElkLogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IReferenceNumberService _referenceNumberService;
        private readonly IAllHeadersPerRequest _allHeadersPerRequest;

        public LoggingBehaviour(IElkLogger<TRequest> logger, ICurrentUserService currentUserService, IReferenceNumberService referenceNumberService, IAllHeadersPerRequest allHeadersPerRequest)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _referenceNumberService = referenceNumberService;
            _allHeadersPerRequest = allHeadersPerRequest;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var SystemId = _currentUserService.SystemCode ?? string.Empty;

            // internal log just log request for new request message
            if (requestName == "PushRequestCommand")
            {
                var obj = request as PushRequestCommand;

                if (string.IsNullOrWhiteSpace(obj.TargetRequest.ContentType))
                {
                    if (obj.TargetRequest.ServiceType == ServiceType.RESTful)
                    {
                        obj.TargetRequest.ContentType = "application/json";
                    }
                    else if (obj.TargetRequest.ServiceType == ServiceType.SOAP)
                    {
                        obj.TargetRequest.ContentType = "text/xml";
                    }
                }

                // generate hash 
                obj.HashObject = new { obj.TargetRequest?.Url, obj.TargetRequest?.ContentBody }.HashObject();

                _logger.LogNewRequest(SystemId, request, _referenceNumberService.ReferenceNumber,
                    _allHeadersPerRequest.Headrs, obj.HashObject,
                    obj.TargetRequest?.Url, obj.CallBackRequest?.Url, obj.TargetRequest.ContentBody, obj.TargetRequest.ContentType);
            }
            await Task.CompletedTask;
        }
    }
}