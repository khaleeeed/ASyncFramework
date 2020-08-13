using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Common.Behaviours
{
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
            var SystemId = _currentUserService.SystemUser ?? string.Empty;

            _logger.LogNewRequest(requestName, SystemId,request, _referenceNumberService.ReferenceNumber,
                _allHeadersPerRequest.Headrs);

            await Task.CompletedTask;
        }
    }
}