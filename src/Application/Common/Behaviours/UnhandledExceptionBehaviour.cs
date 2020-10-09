using ASyncFramework.Application.Common.Exceptions;
using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Domain.Enums;
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
            catch (ValidationException)
            {
                _logger.LogWarning("{Status} {ReferenceNumber}", MessageLifeCycle.ValidationError, _referenceNumberService.ReferenceNumber);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MessageLifeCycle.ExceptoinWhenProcessNewRequest,_referenceNumberService.ReferenceNumber);
                throw;
            }
        }
    }
}
