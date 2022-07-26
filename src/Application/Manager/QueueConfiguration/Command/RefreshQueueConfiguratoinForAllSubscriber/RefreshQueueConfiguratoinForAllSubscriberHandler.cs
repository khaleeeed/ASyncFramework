using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.Manager.QueueConfiguration.Command.RefreshQueueConfiguratoinForAllSubscriber
{
    public class RefreshQueueConfiguratoinForAllSubscriberHandler : IRequestHandler<RefreshQueueConfiguratoinForAllSubscriberCommand, Result>
    {
        private readonly ISubscriberRepository _SubscriberRepository;

        public RefreshQueueConfiguratoinForAllSubscriberHandler(ISubscriberRepository subscriberRepository)
        {
            _SubscriberRepository = subscriberRepository;
        }

        public async Task<Result> Handle(RefreshQueueConfiguratoinForAllSubscriberCommand request, CancellationToken cancellationToken)
        {
            var result = new List<string>();
            var urls=await _SubscriberRepository.GetAllUrl();

            ParallelOptions po = new ParallelOptions() { MaxDegreeOfParallelism = 4 };
            Parallel.ForEach(urls, po, (url, state) =>
             {
                 try
                 {
                     var content = new StringContent("{}", Encoding.UTF8, "application/json");
                     var Url = $"{url}api/Subscriber/RefreshQueueConfiguration";
                     using var clinet = new HttpClient();
                     var respose = clinet.PostAsync(Url, content).Result;
                     if (!respose.IsSuccessStatusCode)
                         result.Add($"{url} not refresh queue configuration");

                 }
                 catch (Exception ex)
                 {
                     result.Add($"{url} not refresh queue configuration");
                 }
             });

            if (result.Count > 0)
                return new Result(false, result);

            return new Result(true, null);
            
        }
    }

    public class RefreshQueueConfiguratoinForAllSubscriberCommand:IRequest<Result>
    {

    }
}
