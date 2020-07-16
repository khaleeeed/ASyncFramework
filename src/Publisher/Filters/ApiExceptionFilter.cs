using ASyncFramework.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            var details = new Result(false, new List<string> { "An error occurred while processing your request.", System.Text.Json.JsonSerializer.Serialize(context.Exception) })
            {
                ReferenceNumber = Guid.NewGuid().ToString()
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }

        private void HandleValidationException(ExceptionContext context)
        {
            var exception = context.Exception as ASyncFramework.Application.Common.Exceptions.ValidationException;

            var details = new Result(false, ConvertDicToList(exception.Errors))
            {
                ReferenceNumber = Guid.NewGuid().ToString()
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
