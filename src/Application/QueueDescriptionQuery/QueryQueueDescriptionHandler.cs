﻿using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.QueryQueueDescription
{    
    public class GetDescriptionQueryHandler : IRequestHandler<GetDescriptionQuery , List<QueueDescription>>
    {
        private readonly IQueueConfigurationService _queueConfiguration;

        public GetDescriptionQueryHandler(IQueueConfigurationService queueConfiguration)
        {
            _queueConfiguration = queueConfiguration;
        }

        public Task<List<QueueDescription>> Handle(GetDescriptionQuery  request, CancellationToken cancellationToken)
        {           
            return Task.FromResult(_queueConfiguration.QueueConfiguration.Where(x=>Regex.IsMatch(x.Key, @"^\d+$")).Select(x => new QueueDescription { ID = x.Key, Name=x.Value.QueueName,Delay=TimeSpan.FromSeconds(x.Value.Dealy/1000).ToString() }).ToList());
        }
    }
    public class GetDescriptionQuery : IRequest<List<QueueDescription>>
    {

    }
}
