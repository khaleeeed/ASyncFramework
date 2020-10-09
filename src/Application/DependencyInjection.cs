using ASyncFramework.Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ASyncFramework.Application.PushRequestLogic;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Application.SubscribeRequestLogic;
using ASyncFramework.Application.Common.Interfaces;
using ASyncFramework.Application.SubscribeRequestLogic.Helper;
using ASyncFramework.Application.Logic;
using ASyncFramework.Application.Manager.MessageFailureQuery.Command.RetryFailuresLogic;

namespace ASyncFramework.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IRetryFailuresMessageLogic,RetryFailuresMessageLogic>();
            services.AddTransient<IQueueLogic, QueueLogic>();
            services.AddTransient<ISendHttpRequest, SendHttpRequest>();
            services.AddTransient<IConvertObjectRequestToHttpRequestMessage, ConvertObjectRequestToHttpRequstMessage>();
            services.AddTransient<IPushRequestLogic,Logic.PushRequestLogic>();
        }
        public static void AddMediatR (this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        }
        public static void AddPublisherMediatR(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        }
      
    }
}
