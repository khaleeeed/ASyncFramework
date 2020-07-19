using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Model;
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
            return _pushRequestLogic.Push(request.PushRequestModel);
        }
    }

    public class PushRequestCommand:IRequest<Result>
    {
        public object MyProperty { get; set; }
        public PushRequest PushRequestModel { get; set; }
    }
    
}
