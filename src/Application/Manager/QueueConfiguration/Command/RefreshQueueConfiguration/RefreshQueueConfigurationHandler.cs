using ASyncFramework.Domain.Model.Response;
using MediatR;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.QueueConfiguration.Command.RefreshQueueConfiguration
{
    public class RefreshQueueConfigurationHandler : IRequestHandler<RefreshQueueConfigurationCommand, Result>
    {
        public async Task<Result> Handle(RefreshQueueConfigurationCommand request, CancellationToken cancellationToken)
        {
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            string url = $"{request.Url}api/Subscriber/RefreshQueueConfiguration";
            using var clinet = new HttpClient();

            var respose= await clinet.PostAsync(url, content);
            if (respose.IsSuccessStatusCode)
                return new Result(true, null);

            return new Result(false, new List<string> { "Subscriber not refresh queue configuration" });
        }
    }

    public class RefreshQueueConfigurationCommand:IRequest<Result>
    {
        public string Url { get; set; }
    }
}
