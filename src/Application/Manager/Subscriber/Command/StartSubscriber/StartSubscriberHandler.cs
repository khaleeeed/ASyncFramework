using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Subscriber.Command.StartSubscriber
{
    public class StartSubscriberHandler : IRequestHandler<StartSubscriberCommand, string>
    {
        public async Task<string> Handle(StartSubscriberCommand request, CancellationToken cancellationToken)
        {
            
            var stringContent = System.Text.Json.JsonSerializer.Serialize(request.TimeStampCheck);
            var content = new StringContent(stringContent, Encoding.UTF8, "application/json");
            string url = $"{request.Url}api/Subscriber/start";
            using var clinet = new HttpClient();
            var respose = await clinet.PostAsync(url, content);
            return await respose.Content.ReadAsStringAsync();
        }
    }
    public class StartSubscriberCommand:IRequest<string>
    {
        public string Url { get; set; }

        public byte[] TimeStampCheck { get; set; }
    }
}
