﻿using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Model;
using System;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface IRabbitProducers
    {
        void PushMessage(Message message, QueueConfigurations queueConfiguration);
    }
}