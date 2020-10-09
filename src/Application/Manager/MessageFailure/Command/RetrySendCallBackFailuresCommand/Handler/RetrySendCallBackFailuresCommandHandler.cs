using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetryFailuresLogic;
using ASyncFramework.Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendCallBackFailuresCommand.Handler
{
    public class RetrySendCallBackFailuresCommandHandler:IRequestHandler<RetrySendCallBackFailureCommand, Result>
    {
        private readonly ICallBackFailureRepository _CallBackFailureRepository;
        private readonly IRetryFailuresMessageLogic _RetryLogic;

        public RetrySendCallBackFailuresCommandHandler(ICallBackFailureRepository callBackFailureRepository, IRetryFailuresMessageLogic retryLogic)
        {
            _CallBackFailureRepository = callBackFailureRepository;
            _RetryLogic = retryLogic;
        }

        public async Task<Result> Handle(RetrySendCallBackFailureCommand request, CancellationToken cancellationToken)
        {
            var isDocExist = await _CallBackFailureRepository.UpdateFaulierProcessing(request.ReferenceNumber);
            if (!isDocExist)
                return new Result(false, new List<string> { "Document not found or in Processing" });

            var doc = await _CallBackFailureRepository.FindDocument(request.ReferenceNumber);
            Result result;
            try
            {
                result = await _RetryLogic.Retry(doc.Fields.Message);
            }
            catch (Exception)
            {
                _ = _CallBackFailureRepository.ReverseFaulierProcessing(request.ReferenceNumber);
                return new Result(false, new List<string> { "message not push" }) { ReferenceNumber = request.ReferenceNumber };
            }
            return result;
        }
    }
    public class RetrySendCallBackFailureCommand:IRequest<Result>
    {
        public string ReferenceNumber { get; set; }
    }
}