using MediatR;
using Microsoft.Extensions.Logging;
using ASyncFramework.Application.Common.Interfaces;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ASyncFramework.Domain.Interface;
using System;

namespace ASyncFramework.Application.Common.Behaviours
{
    /// <summary>
    ///  class will log every request come from Mediatr 
    /// </summary>
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly IInfrastructureLogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IReferenceNumberService _referenceNumberService;

        public PerformanceBehaviour(
            IInfrastructureLogger<TRequest> logger, 
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
            var requestName = typeof(TRequest).Name;

            if (requestName == "PushRequestCommand")
                return await next();

            _timer.Start();

            var response = await next();
           
            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            // internal log if logger take more than 
            if (elapsedMilliseconds > 500)
            {   
                var userId = _currentUserService.SystemCode ?? string.Empty;

                _logger.LogWarning("ASyncFramework.publisher Long Running Request:- RequestName {CreationDate} {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Request} {ReferenceNumber}",
                    DateTime.Now,requestName, elapsedMilliseconds, userId, request,_referenceNumberService.ReferenceNumber);
            }

            return response;
        }
    }
}
