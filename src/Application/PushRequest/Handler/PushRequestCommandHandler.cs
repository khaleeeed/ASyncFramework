using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.Logic;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model;
using ASyncFramework.Domain.Model.Request;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.PushRequestLogic
{   
    public class PushRequestCommandHandler : IRequestHandler<PushRequestCommand, Result>
    {
        private readonly IPushRequestLogic _pushRequestLogic;
        private readonly INotificationRepository _repository;
        private readonly IInfrastructureLogger<PushRequestCommandHandler> _logger;
        private readonly IReferenceNumberService _referenceNumberService;
        private readonly ICurrentUserService _userService;
        private readonly IServiceRepository _serviceRepository;
        private readonly ISystemRepository _systemRepository;

        public PushRequestCommandHandler(IPushRequestLogic pushRequestLogic, INotificationRepository repository, IInfrastructureLogger<PushRequestCommandHandler> logger, IReferenceNumberService referenceNumberService, ICurrentUserService userService, IServiceRepository serviceRepository, ISystemRepository systemRepository)
        {
            _pushRequestLogic = pushRequestLogic;
            _repository = repository;
            _logger = logger;
            _referenceNumberService = referenceNumberService;
            _userService = userService;
            _serviceRepository = serviceRepository;
            _systemRepository = systemRepository;
        }

        public Task<Result> Handle(PushRequestCommand request, CancellationToken cancellationToken)
        {
            // system active service active 
            (bool isSystemActive, bool hasCustomQueue) = _systemRepository.CheckSystemActive(_userService.SystemCode);
            // check if request dispose
            if (!isSystemActive || !_serviceRepository.CheckServiceActive(_userService.ServiceCode))
            {
                _logger.LogFinish(DateTime.Now, MessageLifeCycle.ValidationError, _referenceNumberService.ReferenceNumber);
                _repository.UpdateStatusId(_referenceNumberService.ReferenceNumber, MessageLifeCycle.ValidationError);
                return Task.FromResult(new Result(false, new List<string> { $"the request dispose" }) { ReferenceNumber = _referenceNumberService.ReferenceNumber });
            }

            // check if request send before 
            if (request.IsUniqueRequest)
            {
                (bool IsSendBefore, string ReferenceNumber) = _repository.CheckIfRequestSendBefore(request.HashObject, _referenceNumberService.ReferenceNumber);
                if (IsSendBefore)
                {
                    _logger.LogFinish(DateTime.Now, MessageLifeCycle.ValidationError, _referenceNumberService.ReferenceNumber);
                    _repository.UpdateStatusId(_referenceNumberService.ReferenceNumber, MessageLifeCycle.ValidationError);
                    return Task.FromResult(new Result(false, new List<string> { $"Request Send before with ReferenceNumber: {ReferenceNumber}" }) { ReferenceNumber = _referenceNumberService.ReferenceNumber });
                }
            }
            try
            {
                request.HasCustomQueue = hasCustomQueue;
                _repository.UpdateStatusId(_referenceNumberService.ReferenceNumber, MessageLifeCycle.PushToQueue);
                var pushResult = _pushRequestLogic.Push(request);
                return pushResult;
            }
            catch
            {               
                var message = new List<string> { "Failure push message to queue message maybe take more time" };
                return Task.FromResult(new Result(false, message) { ReferenceNumber = _referenceNumberService.ReferenceNumber });
            }           
        }
    }

    public class PushRequestCommand:IRequest<Result>
    {
        /// <summary>
        /// send queues id sperated by , for queue description call api/message/queue          
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public string Queues { get; set; }

        /// <summary>
        /// Target request service
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public TargetRequestModel TargetRequest { get; set; }

        /// <summary>
        /// CallBack request service most number can send two callback 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public List<CallBackRequestModel> CallBackRequest { get; set; }        
        
        /// <summary>
        /// check if request send before
        /// </summary>
        public bool IsUniqueRequest { get; set; }
        /// <summary>
        /// ExtraInfoForSearch
        /// </summary>
        public string ExtraInfo { get; set; }
        /// <summary>
        /// set in LoggingBehaviour 
        /// </summary>
        internal string HashObject { get; set; }

        internal bool HasCustomQueue { get; set; }
    }    
}