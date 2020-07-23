using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Application.PushRequestLogic
{
    public class PushRequestValidator : AbstractValidator<PushRequestCommand>
    {
        public PushRequestValidator()
        {
            RuleFor(x => x).NotNull().DependentRules(() =>
            {
                RuleFor(x=>x.Queues).Matches(@"\d{1,2}(,\d{1,2})*$").WithMessage("Queues format *,*,*");

                RuleFor(x=>x.TargetRequest).NotNull().DependentRules(()=> 
                {
                    RuleFor(x => x).CustomAsync((x, c, ct) => SetContentType(x.TargetRequest,c,ct));
                    RuleFor(x=>x.TargetRequest.Url).Must(tu => Uri.IsWellFormedUriString(tu, UriKind.RelativeOrAbsolute)).WithMessage("TargetRequest.Url required with uriFormat");
                    RuleFor(x => x.TargetRequest.SoapAction).NotEmpty().When(x => x.TargetRequest.ServiceType == Domain.Enums.ServiceType.SOAP).WithMessage("TargetRequest.SoapAction required when send SOAP request");
                    RuleFor(x => x.TargetRequest.MethodVerb).Equal(Domain.Enums.MethodVerb.Post).When(x => x.TargetRequest.ServiceType == Domain.Enums.ServiceType.SOAP).WithMessage("TargetRequest.MethodVerb must equal post when send SOAP request");

                }).WithMessage("TargetRequest required");

                RuleFor(x => x.TargetOAuthRequest).CustomAsync((x, c, ct) => SetContentTypeForOAuthApi(x, c, ct));
                RuleFor(x => x.TargetOAuthRequest.Url).Must(tu => Uri.IsWellFormedUriString(tu, UriKind.RelativeOrAbsolute)).When(x => x.TargetOAuthRequest != null).WithMessage("TargetOAuthRequest.Url required with uriFormat");
                RuleFor(x => x.TargetOAuthRequest.ServiceType).Equal(Domain.Enums.ServiceType.RESTful).When(x => x.TargetOAuthRequest != null).WithMessage("TargetOAuthRequest.ServiceType must equal RESTful");


                RuleFor(x => x.CallBackRequest).NotNull().DependentRules(() =>
                {
                    RuleFor(x => x).CustomAsync((x, c, ct) => SetContentType(x.CallBackRequest, c, ct));
                    RuleFor(x => x.CallBackRequest.Url).Must(tu => Uri.IsWellFormedUriString(tu, UriKind.RelativeOrAbsolute)).WithMessage("CallBackRequest.Url required with uriFormat");
                    RuleFor(x => x.CallBackRequest.SoapAction).NotEmpty().When(x => x.TargetRequest.ServiceType == Domain.Enums.ServiceType.SOAP).WithMessage("CallBackRequest.SoapAction required when send SOAP request");

                }).WithMessage("CallBackRequest required");

                RuleFor(x => x.CallBackOAuthRequest).CustomAsync((x, c, ct) => SetContentTypeForOAuthApi(x, c, ct));
                RuleFor(x => x.CallBackOAuthRequest.Url).Must(tu => Uri.IsWellFormedUriString(tu, UriKind.RelativeOrAbsolute)).When(x => x.CallBackOAuthRequest != null).WithMessage("CallBackOAuthRequest.Url required with uriFormat");
                RuleFor(x => x.CallBackOAuthRequest.ServiceType).Equal(Domain.Enums.ServiceType.RESTful).When(x => x.CallBackOAuthRequest != null).WithMessage("CallBackOAuthRequest.ServiceType must equal RESTful");

            }).WithMessage("PushRequestCommand required");
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
