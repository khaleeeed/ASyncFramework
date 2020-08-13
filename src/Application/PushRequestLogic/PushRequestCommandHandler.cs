using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Model;
using ASyncFramework.Domain.Model.Request;
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

        public PushRequestCommandHandler(IPushRequestLogic pushRequestLogic)
        {
            _pushRequestLogic = pushRequestLogic;
        }

        public Task<Result> Handle(PushRequestCommand request, CancellationToken cancellationToken)
        {
            return _pushRequestLogic.Push(request);
        }
    }

    public class PushRequestCommand:IRequest<Result>
    {
        /// <summary>
        /// send queues id sperated by , for queue description call api/message/queue          
        /// </summary>
        public string Queues { get; set; }

        public Request TargetOAuthRequest { get; set; }
        public PushRequest TargetRequest { get; set; }
        public CallBackRequest CallBackRequest { get; set; }
        public Request CallBackOAuthRequest { get; set; }
    }
    
}
