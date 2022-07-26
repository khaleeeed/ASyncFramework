using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.PushRequestLogic
{

    /// <summary>
    /// PushRequestCommand validator call from midetor ValidationBehavior
    /// </summary>
    public class PushRequestValidator : AbstractValidator<PushRequestCommand>
    {
        public PushRequestValidator(IQueueConfigurationService queueConfiguration)
        {
            // PushRequestCommand object required 
            RuleFor(x => x).NotNull().DependentRules(() =>
            {
                // Queues must be number and comma separated 
                RuleFor(x=>x.Queues).NotEmpty().Matches(@"\d{1,2}(,\d{1,2})*$").DependentRules(()=> 
                {
                    // check if all queue present in system 
                    RuleFor(x => x.Queues).CustomAsync((x, c, ct) => CheckAllQueuepresent(x, c, ct, queueConfiguration));

                }).WithMessage("Queues format *,*,*");                

                // targetRequest object required
                RuleFor(x=>x.TargetRequest).NotNull().DependentRules(()=>
                {
                    RuleFor(x => x.TargetRequest.TargetServiceRequest).NotNull().DependentRules(() => 
                    {
                        // check if enum is not in range 
                        RuleFor(x => x.TargetRequest.TargetServiceRequest.MethodVerb).IsInEnum();

                        // check ContentType if null set default
                        RuleFor(x => x).CustomAsync((x, c, ct) => SetContentType(x.TargetRequest.TargetServiceRequest, c, ct));

                        // url property must url format
                        RuleFor(x => x.TargetRequest.TargetServiceRequest.Url).Must(tu => Uri.IsWellFormedUriString(tu, UriKind.Absolute)).WithMessage("TargetRequest.Url required with uriFormat");
                        RuleFor(x => x.TargetRequest.TargetServiceRequest).CustomAsync((x, c, ct) => CheckUrl(x, c, ct));

                        // url containst async 
                        RuleFor(x => x.TargetRequest.TargetServiceRequest.Url).Must(t => !t.Contains("ASyncFramework.")).WithMessage("Cannot use ASync api as TargetRequest.url");
                        RuleFor(x => x.TargetRequest.TargetServiceRequest.Url).Must(t => !t.Contains("10.162.1.164")).WithMessage("Cannot use ASync api as TargetRequest.url");

                        // SoapAction required when ServiceType = SOAP
                        RuleFor(x => x.TargetRequest.TargetServiceRequest.SoapAction).NotEmpty().When(x => x.TargetRequest.TargetServiceRequest.ServiceType == Domain.Enums.ServiceType.SOAP).WithMessage("TargetRequest.SoapAction required when send SOAP request");

                        // MethodVerb must be Post when ServiceType = SOAP
                        RuleFor(x => x.TargetRequest.TargetServiceRequest.MethodVerb).Equal(Domain.Enums.MethodVerb.Post).When(x => x.TargetRequest.TargetServiceRequest.ServiceType == Domain.Enums.ServiceType.SOAP).WithMessage("TargetRequest.MethodVerb must equal post when send SOAP request");
                    }).WithMessage("TargetServiceRequest required");

                }).WithMessage("TargetRequest required");

                // CallBackRequest object rquired 
                RuleFor(x => x.CallBackRequest).Must(x => x.Count < 3).When(x => x.CallBackRequest != null).DependentRules(() =>
                {
                    // check ContentType if null set default
                    RuleForEach(x => x.CallBackRequest).CustomAsync((x, c, ct) => SetContentType(x.CallBackServiceRequest, c, ct));

                    // url property must url format 
                    RuleForEach(x => x.CallBackRequest).Must(tu => Uri.IsWellFormedUriString(tu.CallBackServiceRequest.Url, UriKind.Absolute)).When(x => x.CallBackRequest != null).WithMessage("CallBackRequest.Url required with uriFormat");

                    // url containst async
                    RuleForEach(x => x.CallBackRequest).ChildRules(child=>child.RuleFor(x=>x.CallBackServiceRequest).Must(t => !t.Url.Contains("ASyncFramework.")).When(t=> t.CallBackServiceRequest.Url != null)).When(x => x.CallBackRequest != null).WithMessage("Cannot use ASync api as CallBackRequest.url");
                    RuleForEach(x => x.CallBackRequest).ChildRules(child => child.RuleFor(x => x.CallBackServiceRequest).Must(t => !t.Url.Contains("10.162.1.164")).When(t => t.CallBackServiceRequest.Url != null)).When(x => x.CallBackRequest != null).WithMessage("Cannot use ASync api as CallBackRequest.url");

                    RuleForEach(x => x.CallBackRequest).CustomAsync((x, c, ct) => CheckUrl(x.CallBackServiceRequest, c, ct));

                    // SoapAction required when ServiceType = SOAP
                    RuleForEach(x => x.CallBackRequest).ChildRules(child => child.RuleFor(x => x.CallBackServiceRequest.SoapAction).NotEmpty().When(x=>x.CallBackServiceRequest.ServiceType == Domain.Enums.ServiceType.SOAP).WithMessage("CallBackRequest.SoapAction required when send SOAP request"));
          
                
                }).WithMessage("callback service shouldn't greater than 2");

       
            }).WithMessage("PushRequestCommand required");
        }

        private Task CheckAllQueuepresent(string stringQueue, CustomContext c, CancellationToken ct, IQueueConfigurationService value)
        {
            var queues = stringQueue.Split(",");

            foreach (var queue in queues)
            {
                
                if (!value.QueueConfiguration.ContainsKey(queue))
                {
                    c.AddFailure($"{queue} Queue is not present in the system");
                }
            }

            var duplicates = new Dictionary<string,int>(); 
            foreach (var queue in queues)
            {
                if (duplicates.ContainsKey(queue))
                {
                    duplicates[queue]++;
                }
                else
                    duplicates.Add(queue, 0);
            }

            foreach (var queue in duplicates)
            {
                if (queue.Value>3)
                {
                    c.AddFailure($"queue {queue.Key} can use 3 time at the most.you use it {queue.Value} times");
                }
            }

            return Task.CompletedTask;
        }

        private Task SetContentType(Domain.Model.Request.BaseRequest request,CustomContext c,CancellationToken ct)
        {
            if (request == null)
            {
                return Task.CompletedTask;
            }
            
            if (string.IsNullOrWhiteSpace(request.ContentType))
            {
                if (request.ServiceType==Domain.Enums.ServiceType.RESTful)
                {
                    request.ContentType = "application/json";
                }
                else if (request.ServiceType==Domain.Enums.ServiceType.SOAP)
                {
                    request.ContentType = "text/xml";
                }
            }
            else
            {
                try
                {
                    if (!MediaTypeWithQualityHeaderValue.TryParse(request.ContentType, out _))
                        c.AddFailure($"{c.PropertyName}.ContentType media type ({request.ContentType}) is invalid.");
                    else
                        new System.Net.Http.StringContent(string.Empty, Encoding.UTF8, request.ContentType);
                }
                catch
                {
                    c.AddFailure($"{c.PropertyName}.ContentType media type ({request.ContentType}) is invalid.");
                }
            }
            return Task.CompletedTask;
        }

        private Task SetContentTypeForOAuthApi(Domain.Model.Request.BaseRequest request,CustomContext c, CancellationToken ct)
        {
            if (request != null)
                SetContentType(request, c, ct);

            return Task.CompletedTask;
        }

        private Task CheckUrl(Domain.Model.Request.BaseRequest request, CustomContext c, CancellationToken ct)
        {
            if (request == null)
                return Task.CompletedTask;
            try
            {
                new HttpRequestMessage(HttpMethod.Get, request.Url);
            }
            catch
            {
                c.AddFailure($"{c.PropertyName}.url ({request.Url}) is invalid.");
            }
 
            return Task.CompletedTask;
        }
    }
}