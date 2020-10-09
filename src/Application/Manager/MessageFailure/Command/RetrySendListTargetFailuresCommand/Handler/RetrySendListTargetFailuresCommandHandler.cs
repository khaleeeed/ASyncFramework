using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetryFailuresLogic;
using ASyncFramework.Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetrySendListTargetFailuresCommand.Handler
{
    public class RetrySendListTargetFailuresCommandHandler : IRequestHandler<RetrySendListTargetFailuresCommand, Result>
    {
        private readonly ITargetFailuerRepository _TargetFailureRepository;
        private readonly IRetryFailuresMessageLogic _RetryLogic;

        public RetrySendListTargetFailuresCommandHandler(ITargetFailuerRepository targetFailureRepository, IRetryFailuresMessageLogic retryLogic)
        {
            _TargetFailureRepository = targetFailureRepository;
            _RetryLogic = retryLogic;
        }
        public async Task<Result> Handle(RetrySendListTargetFailuresCommand request, CancellationToken cancellationToken)
        {
            var isDocExist = await _TargetFailureRepository.UpdateFaulierProcessing(request.ReferenceNumbers);
            if (!isDocExist)
                return new Result(false, new List<string> { "Document not found or in Processing" });

            List<string> erros = null;
            Result result = new Result(true, null);

            foreach (var item in request.ReferenceNumbers)
            {
                var doc = await _TargetFailureRepository.FindDocument(item);

                if (doc == null)
                    continue;
                try
                {
                    _=await _RetryLogic.Retry(doc.Fields.Message);
                }
                catch (Exception ex)
                {
                    erros ??= new List<string>();
                    erros.Add($"{item} not push");
                    _=_TargetFailureRepository.ReverseFaulierProcessing(item);
                }
            }
            result.Errors = erros?.ToArray();
            return result;
        }
    }
    public class RetrySendListTargetFailuresCommand : IRequest<Result>
    {
        public List<string> ReferenceNumbers { get; set; }
    }
}
