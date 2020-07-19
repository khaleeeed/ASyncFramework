using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Application.PushRequestLogic
{
    public class PushRequestValidator : AbstractValidator<PushRequestCommand>
    {
        public PushRequestValidator()
        {
            RuleFor(x => x).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.PushRequestModel).NotNull().DependentRules(() =>
                {
                    RuleFor(x => x.PushRequestModel.CallBackUrl).Must(cb => Uri.IsWellFormedUriString(cb, UriKind.RelativeOrAbsolute)).WithMessage("CallBackUri required with uriFormat");
                    RuleFor(x => x.PushRequestModel.Queue).Matches(@"\d{1,2}(,\d{1,2})*$").WithMessage("Queue format *,*,*");
                    RuleFor(x => x.PushRequestModel.TargetUrl).Must(tu => Uri.IsWellFormedUriString(tu, UriKind.RelativeOrAbsolute)).WithMessage("TargetUrl required with uriFormat");
                    RuleFor(x => x.PushRequestModel.OAuthHttpCode).NotEmpty().WithMessage("OAuthHttpCode required");
                }).WithMessage("PushRequestModel required");
            }).WithMessage("PushRequestCommand required");
        }
    }
}
