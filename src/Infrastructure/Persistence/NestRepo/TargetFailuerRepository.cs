using ASyncFramework.Domain.Documents;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure.Persistence.NestRepo.Configurations;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Infrastructure.Persistence.NestRepo
{
    public class TargetFailuerRepository: GenericFaiulerRepository<TargetFailuerDocument>, ITargetFailuerRepository
    {
        public override string IndexName => "asyncframework-targetfailuer-";

        public TargetFailuerRepository(IElasticClient client):base(client)
        {
        }

    }
}
