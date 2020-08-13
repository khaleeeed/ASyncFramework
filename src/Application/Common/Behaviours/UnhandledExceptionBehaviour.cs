using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Common.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IElkLogger<TRequest> _logger;
        private readonly IReferenceNumberService _referenceNumberService;

        public UnhandledExceptionBehaviour(IElkLogger<TRequest> logger,IReferenceNumberService referenceNumberService)
        {
            _referenceNumberService = referenceNumberService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogError(ex, "ASyncFramework Request: Unhandled Exception for Request {Name} {Request} {ReferenceNumber}", requestName, request,_referenceNumberService.ReferenceNumber);

                throw;
            }
        }
    }
}
