using MediatR;
using Microsoft.Extensions.Logging;
using ASyncFramework.Application.Common.Interfaces;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ASyncFramework.Domain.Interface;

namespace ASyncFramework.Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly IElkLogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IReferenceNumberService _referenceNumberService;

        public PerformanceBehaviour(
            IElkLogger<TRequest> logger, 
            ICurrentUserService currentUserService,
            IReferenceNumberService referenceNumberService)
        {
            _timer = new Stopwatch();
            _referenceNumberService = referenceNumberService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            var response = await next();
           
            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = _currentUserService.SystemUser ?? string.Empty;

                _logger.LogWarning("ASyncFramework.publisher Long Running Request:- RequestName {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Request} {ReferenceNumber}",
                    requestName, elapsedMilliseconds, userId, request,_referenceNumberService.ReferenceNumber);
            }

            return response;
        }
    }
}
