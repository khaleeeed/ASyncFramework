using ASyncFramework.Application.Common.Hashing;
using ASyncFramework.Application.PushRequestLogic;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using MediatR.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Common.Behaviours
{
    /// <summary>
    ///  class will log every request come from Mediatr 
    /// </summary>
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly IInfrastructureLogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IReferenceNumberService _referenceNumberService;
        private readonly IAllHeadersPerRequest _allHeadersPerRequest;
        private readonly INotificationRepository _notificationRepository;

        public LoggingBehaviour(IInfrastructureLogger<TRequest> logger, ICurrentUserService currentUserService, IReferenceNumberService referenceNumberService, IAllHeadersPerRequest allHeadersPerRequest, INotificationRepository notificationRepository)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _referenceNumberService = referenceNumberService;
            _allHeadersPerRequest = allHeadersPerRequest;
            _notificationRepository = notificationRepository;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var SystemId = _currentUserService.SystemCode ?? string.Empty;

            // internal log just log request for new request message
            if (requestName == "PushRequestCommand")
            {
                var obj = request as PushRequestCommand;

                // set default ContentType for request 
                if (string.IsNullOrWhiteSpace(obj.TargetRequest.TargetServiceRequest.ContentType))
                {
                    if (obj.TargetRequest.TargetServiceRequest.ServiceType == ServiceType.RESTful)
                    {
                        obj.TargetRequest.TargetServiceRequest.ContentType = "application/json";
                    }
                    else if (obj.TargetRequest.TargetServiceRequest.ServiceType == ServiceType.SOAP)
                    {
                        obj.TargetRequest.TargetServiceRequest.ContentType = "text/xml";
                    }
                }

                // generate hash 
                obj.HashObject = new { obj.TargetRequest?.TargetServiceRequest.Url, obj.TargetRequest?.TargetServiceRequest.ContentBody }.HashObject();

                string callbackUrl = string.Empty;
                obj.CallBackRequest?.ForEach(x => callbackUrl = callbackUrl + " " + x.CallBackServiceRequest.Url);

                try
                {
                    // insert new request in slq dataBase 
                    long notifcationId = _notificationRepository.Add(new Domain.Entities.NotificationEntity
                    {
                        SystemCode = SystemId,
                        Request = System.Text.Json.JsonSerializer.Serialize(obj),
                        CallBackUrl = callbackUrl,
                        CreationDate = DateTime.Now,
                        ExtraInfo = obj.ExtraInfo,
                        Hash = obj.HashObject,
                        Header = System.Text.Json.JsonSerializer.Serialize(_allHeadersPerRequest.Headrs),
                        TargetUrl = obj.TargetRequest?.TargetServiceRequest.Url,
                        ServiceCode= _currentUserService.ServiceCode
                    });
                   // set refrence number at scoped register class
                    _referenceNumberService.ReferenceNumber = notifcationId.ToString();

                    // log in elk for new reuqest 
                    _logger.LogNewRequest(CreationDate: DateTime.Now, systemId: SystemId, referenceNumber: _referenceNumberService.ReferenceNumber,
                    headrs: _allHeadersPerRequest.Headrs, targetUrl: obj.TargetRequest?.TargetServiceRequest.Url, callBackUrl: callbackUrl,
                   Content: obj.TargetRequest.TargetServiceRequest.ContentBody, ContentType: obj.TargetRequest.TargetServiceRequest.ContentType);

                }
                catch (Exception ex)
                {
                    _logger.LogError(DateTime.Now,ex, MessageLifeCycle.NewRequest, _referenceNumberService.ReferenceNumber);
                    throw;
                }
            }
            await Task.CompletedTask;
        }
    }
}