﻿using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Subscriber.Command.ForceStopSubscriber
{
    public class ForceStopSubscriberHandler : IRequestHandler<ForceStopSubscriberCommand, string>
    {
        public async Task<string> Handle(ForceStopSubscriberCommand request, CancellationToken cancellationToken)
        {
            var stringContent = System.Text.Json.JsonSerializer.Serialize(request.TimeStampCheck);
            var content = new StringContent(stringContent, Encoding.UTF8, "application/json");
            string url = $"{request.Url}api/Subscriber/ForceStop";
            using var clinet = new HttpClient();
            var respose = await clinet.PostAsync(url, content);

            return await respose.Content.ReadAsStringAsync();
        }
    }
    public class ForceStopSubscriberCommand:IRequest<string>
    {
        public string Url { get; set; }

        public byte[] TimeStampCheck { get; set; }
    }
}
