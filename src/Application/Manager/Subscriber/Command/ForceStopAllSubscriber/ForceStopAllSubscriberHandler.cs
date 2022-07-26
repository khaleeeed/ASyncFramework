using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.Subscriber.Command.ForceStopAllSubscriber
{
    public class ForceStopAllSubscriberHandler : IRequestHandler<ForceStopAllSubscriberCommand, Result>
    {
        public Task<Result> Handle(ForceStopAllSubscriberCommand request, CancellationToken cancellationToken)
        {
            var result = new List<string>();

            for (int i = 0; i < request.Urls.Count; i++)
            {
                int index = Convert.ToInt32(i);
                try
                {
                    var timeStampChecks = request.TimeStampChecks[index];
                    var stringContent = System.Text.Json.JsonSerializer.Serialize(timeStampChecks);
                    var content = new StringContent(stringContent, Encoding.UTF8, "application/json");
                    string Url = $"{request.Urls[index]}api/Subscriber/ForceStop";
                    using var clinet = new HttpClient();
                    var respose = clinet.PostAsync(Url, content).Result;
                    if (!respose.IsSuccessStatusCode)
                        result.Add($"{request.Urls[index]} not stop");

                }
                catch (Exception ex)
                {
                    result.Add($"{request.Urls[index]} not stop");
                }
            }

            if (result.Count > 0)
                return Task.FromResult(new Result(false, result));

            return Task.FromResult(new Result(true, null));
        }
    }

    public class ForceStopAllSubscriberCommand:IRequest<Result>
    {
        public List<string> Urls { get; set; }
        public List<byte[]> TimeStampChecks { get; set; }
    }

}
