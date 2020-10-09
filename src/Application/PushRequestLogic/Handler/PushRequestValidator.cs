using ASyncFramework.Domain.Common;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public PushRequestValidator(IOptions<Dictionary<string, QueueConfiguration>> queueConfiguration)
        {
            // PushRequestCommand object required 
            RuleFor(x => x).NotNull().DependentRules(() =>
            {
                // Queues must be number and comma separated  
                RuleFor(x=>x.Queues).NotEmpty().Matches(@"\d{1,2}(,\d{1,2})*$").DependentRules(()=> 
                {
                    // check if all queue present in system 
                    RuleFor(x => x.Queues).CustomAsync((x, c, ct) => CheckAllQueuepresent(x, c, ct, queueConfiguration.Value));

                }).WithMessage("Queues format *,*,*");                

                // targetRequest object required
                RuleFor(x=>x.TargetRequest).NotNull().DependentRules(()=> 
                {
                    // check if enum is not in range 
                    RuleFor(x => x.TargetRequest.MethodVerb).IsInEnum();

                    // check ContentType if null set default
                    RuleFor(x => x).CustomAsync((x, c, ct) => SetContentType(x.TargetRequest,c,ct));
                   
                    // url property must url format 
                    RuleFor(x=>x.TargetRequest.Url).Must(tu => Uri.IsWellFormedUriString(tu, UriKind.Absolute)).WithMessage("TargetRequest.Url required with uriFormat");

                    // url containst async 
                    RuleFor(x => x.TargetRequest.Url).Must(t => !t.Contains("ASyncFramework.")).WithMessage("Cannot use ASync api as TargetRequest.url");
                    RuleFor(x => x.TargetRequest.Url).Must(t => !t.Contains("async/api")).WithMessage("Cannot use ASync api as TargetRequest.url");

                    // SoapAction required when ServiceType = SOAP
                    RuleFor(x => x.TargetRequest.SoapAction).NotEmpty().When(x => x.TargetRequest.ServiceType == Domain.Enums.ServiceType.SOAP).WithMessage("TargetRequest.SoapAction required when send SOAP request");

                    // MethodVerb must be Post when ServiceType = SOAP
                    RuleFor(x => x.TargetRequest.MethodVerb).Equal(Domain.Enums.MethodVerb.Post).When(x => x.TargetRequest.ServiceType == Domain.Enums.ServiceType.SOAP).WithMessage("TargetRequest.MethodVerb must equal post when send SOAP request");

                }).WithMessage("TargetRequest required");

                //check ContentType if null set default
                RuleFor(x => x.TargetOAuthRequest).CustomAsync((x, c, ct) => SetContentTypeForOAuthApi(x, c, ct));

                // check if enum is not in range 
                RuleFor(x => x.TargetOAuthRequest.MethodVerb).IsInEnum().When(x => x.TargetOAuthRequest != null);

                // url property must url format when TargetOAuthRequest not null
                RuleFor(x => x.TargetOAuthRequest.Url).Must(tu => Uri.IsWellFormedUriString(tu, UriKind.Absolute)).When(x => x.TargetOAuthRequest != null).WithMessage("TargetOAuthRequest.Url required with uriFormat");

                // url containst async 
                RuleFor(x => x.TargetOAuthRequest.Url).Must(t => !t.Contains("ASyncFramework.")).When(x => x.TargetOAuthRequest != null).WithMessage("Cannot use ASync api as TargetOAuthRequest.url");
                RuleFor(x => x.TargetOAuthRequest.Url).Must(t => !t.Contains("async/api")).When(x => x.TargetOAuthRequest != null).WithMessage("Cannot use ASync api as TargetOAuthRequest.url");

                RuleFor(x => x.TargetOAuthRequest.ServiceType).Equal(Domain.Enums.ServiceType.RESTful).When(x => x.TargetOAuthRequest != null).WithMessage("TargetOAuthRequest.ServiceType must equal RESTful");

                // CallBackRequest object rquired 
                RuleFor(x => x.CallBackRequest).NotNull().DependentRules(() =>
                {
                    // check ContentType if null set default
                    RuleFor(x => x).CustomAsync((x, c, ct) => SetContentType(x.CallBackRequest, c, ct));

                    // url property must url format 
                    RuleFor(x => x.CallBackRequest.Url).Must(tu => Uri.IsWellFormedUriString(tu, UriKind.Absolute)).WithMessage("CallBackRequest.Url required with uriFormat");

                    // url containst async 
                    RuleFor(x => x.CallBackRequest.Url).Must(t => !t.Contains("ASyncFramework.")).WithMessage("Cannot use ASync api as CallBackRequest.url");
                    RuleFor(x => x.CallBackRequest.Url).Must(t => !t.Contains("async/api")).WithMessage("Cannot use ASync api as CallBackRequest.url");

                    // SoapAction required when ServiceType = SOAP
                    RuleFor(x => x.CallBackRequest.SoapAction).NotEmpty().When(x => x.TargetRequest.ServiceType == Domain.Enums.ServiceType.SOAP).WithMessage("CallBackRequest.SoapAction required when send SOAP request");

                }).WithMessage("CallBackRequest required");

                // check if enum is not in range 
                RuleFor(x => x.CallBackOAuthRequest.MethodVerb).IsInEnum().When(x => x.CallBackOAuthRequest != null);

                //check ContentType if null set default
                RuleFor(x => x.CallBackOAuthRequest).CustomAsync((x, c, ct) => SetContentTypeForOAuthApi(x, c, ct));

                // url property must url format when TargetOAuthRequest not null
                RuleFor(x => x.CallBackOAuthRequest.Url).Must(tu => Uri.IsWellFormedUriString(tu, UriKind.Absolute)).When(x => x.CallBackOAuthRequest != null).WithMessage("CallBackOAuthRequest.Url required with uriFormat");

                // url containst async 
                RuleFor(x => x.CallBackOAuthRequest.Url).Must(t => !t.Contains("ASyncFramework.")).When(x => x.CallBackOAuthRequest != null).WithMessage("Cannot use ASync api as CallBackOAuthRequest.url");
                RuleFor(x => x.CallBackOAuthRequest.Url).Must(t => !t.Contains("async/api")).When(x => x.CallBackOAuthRequest != null).WithMessage("Cannot use ASync api as CallBackOAuthRequest.url");


                RuleFor(x => x.CallBackOAuthRequest.ServiceType).Equal(Domain.Enums.ServiceType.RESTful).When(x => x.CallBackOAuthRequest != null).WithMessage("CallBackOAuthRequest.ServiceType must equal RESTful");

            }).WithMessage("PushRequestCommand required");
        }

        private Task CheckAllQueuepresent(string stringQueue, CustomContext c, CancellationToken ct, Dictionary<string, QueueConfiguration> value)
        {
            var queues = stringQueue.Split(",");

            foreach (var queue in queues)
            {
                if (!value.ContainsKey(queue))
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
                if (!MediaTypeWithQualityHeaderValue.TryParse(request.ContentType, out _))
                    c.AddFailure($"{c.PropertyName}.ContentType media type ({request.ContentType}) is invalid.");
            }
            return Task.CompletedTask;
        }

        private Task SetContentTypeForOAuthApi(Domain.Model.Request.BaseRequest request,CustomContext c, CancellationToken ct)
        {
            if (request != null)
                SetContentType(request, c, ct);

            return Task.CompletedTask;
        }
    }
}
