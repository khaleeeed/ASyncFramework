using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure.Persistence.NestRepo.Configurations;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Infrastructure.Persistence.NestRepo
{
    public class CallBackFailureRepository:GenericFaiulerRepository<CallBackFailuerDocument>, ICallBackFailureRepository
    {        
        public override string IndexName => "asyncframework-callbackfailure-";
        public CallBackFailureRepository(IElasticClient client):base(client)
        {
        }
    }
}
