using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.Logic;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetryFailuresLogic;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendTargetFailureCommand.Handler
{
    public class RetrySendTargetFailureCommandHandler : IRequestHandler<RetrySendTargetFailureCommand, Result>
    {
        private readonly ITargetFailuerRepository _TargetFailureRepository;
        private readonly IRetryFailuresMessageLogic _RetryLogic;

        public RetrySendTargetFailureCommandHandler(ITargetFailuerRepository targetFailureRepository, IRetryFailuresMessageLogic retryLogic)
        {
            _TargetFailureRepository = targetFailureRepository;
            _RetryLogic = retryLogic;
        }

        public async Task<Result> Handle(RetrySendTargetFailureCommand request, CancellationToken cancellationToken)
        {
            var isDocExist = await _TargetFailureRepository.UpdateFaulierProcessing(request.ReferenceNumber,request.TimeStampCheck);
            if (!isDocExist)
                return new Result(false, new List<string> { "Document not found or in Processing" });

            var doc = await _TargetFailureRepository.FindDocument(request.ReferenceNumber);
            Result result;
            try
            {
                result =await _RetryLogic.Retry(doc.Message);
            }
            catch (Exception)
            {
                _ = _TargetFailureRepository.ReverseFaulierProcessing(request.ReferenceNumber);
                return new Result(false, new List<string> { "message not push" }) { ReferenceNumber = request.ReferenceNumber };
            }
            return result;
        }
    }
    public class RetrySendTargetFailureCommand:IRequest<Result>
    {
        public string ReferenceNumber { get; set; }
        public byte[] TimeStampCheck { get; set; }
    }
}
