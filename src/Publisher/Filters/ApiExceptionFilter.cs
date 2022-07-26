using ASyncFramework.Application.Common.Models;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Publisher.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {

        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilter()
        {
            // Register known exception types and handlers.
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ASyncFramework.Application.Common.Exceptions.ValidationException), HandleValidationException }
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);

            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            IReferenceNumberService referenceNumber = context?.HttpContext?.RequestServices?.GetService<IReferenceNumberService>();

            var exceptionString = Newtonsoft.Json.JsonConvert.SerializeObject(context.Exception);
            var details = new Result(false, new List<string> { "An error occurred while processing your request.",exceptionString  })
            {
                ReferenceNumber = referenceNumber?.ReferenceNumber
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };  

            context.ExceptionHandled = true;
        }

        private void HandleValidationException(ExceptionContext context)
        {
            IReferenceNumberService referenceNumber = context?.HttpContext?.RequestServices?.GetService<IReferenceNumberService>();

            var repository = context?.HttpContext?.RequestServices?.GetService<INotificationRepository>();
            repository.UpdateStatusId(referenceNumber.ReferenceNumber, MessageLifeCycle.ValidationError);

            var exception = context.Exception as ASyncFramework.Application.Common.Exceptions.ValidationException;

            var details = new Result(false, ConvertDicToList(exception.Errors))
            {
                ReferenceNumber = referenceNumber?.ReferenceNumber
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity
            };

            context.ExceptionHandled = true;
        }

        private List<string> ConvertDicToList (IDictionary<string,string[]> Dictionary)
        {
            List<string> list = new List<string>();
            foreach (var keyValue in Dictionary)
            {
                foreach (var value in keyValue.Value)
                {
                    list.Add(value);
                }
            }
            return list;
        }
     
    }
}
