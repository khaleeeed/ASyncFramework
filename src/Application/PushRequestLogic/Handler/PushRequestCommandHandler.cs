using ASyncFramework.Application.Common.Models;
using ASyncFramework.Application.Logic;
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
        [System.ComponentModel.DataAnnotations.Required]
        public string Queues { get; set; }

        /// <summary>
        /// required if target api had token 
        /// token must retrieve in property name access_Token 
        /// </summary>
        public Request TargetOAuthRequest { get; set; }

        /// <summary>
        /// target api 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public PushRequest TargetRequest { get; set; }

        /// <summary>
        /// call back api
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public CallBackRequest CallBackRequest { get; set; }

        /// <summary>
        /// required if call back api had token 
        /// token must retrieve in property name access_Token 
        /// </summary>
        public Request CallBackOAuthRequest { get; set; }
    }
    
}
