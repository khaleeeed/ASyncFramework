using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetryFailuresLogic;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendListCallBackFailureCommand.Handler
{
    public class RetrySendListCallBackFailureCommandHandler : IRequestHandler<RetrySendListCallBackFailuresCommand, Result>
    {
        private readonly ICallBackFailureRepository _CallBackFailureRepository;
        private readonly IRetryFailuresMessageLogic _RetryLogic;



        public RetrySendListCallBackFailureCommandHandler(ICallBackFailureRepository callBackFailureRepository, IRetryFailuresMessageLogic retryLogic)
        {
            _CallBackFailureRepository = callBackFailureRepository;
            _RetryLogic = retryLogic;
        }

        public async Task<Result> Handle(RetrySendListCallBackFailuresCommand request, CancellationToken cancellationToken)
        {
            var isDocUpdated = await _CallBackFailureRepository.UpdateFaulierProcessing(request.ReferenceNumbers,request.TimeStampChecks);
            if (isDocUpdated != null)
                return new Result(false, new List<string> { $"Document {isDocUpdated} in Processing please refresh page" });

            List<string> erros = null;
            Result result = new Result(true, null);

            foreach (var item in request.ReferenceNumbers)
            {
                var doc = await _CallBackFailureRepository.FindDocument(item);

                if (doc == null)
                    continue;
                try
                {
                    _ = await _RetryLogic.Retry(doc.Message);
                }
                catch (Exception ex)
                {
                    erros ??= new List<string>();
                    erros.Add($"{item} not push");
                    _ = _CallBackFailureRepository.ReverseFaulierProcessing(item);
                }
            }
            result.Errors = erros?.ToArray();
            return result;
        }
    }
    public class RetrySendListCallBackFailuresCommand : IRequest<Result>
    {
        public List<string> ReferenceNumbers { get; set; }
        public List<byte[]> TimeStampChecks { get; set; }
    }
}