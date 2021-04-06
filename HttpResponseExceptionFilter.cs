using Battleships.Errors;
using Battleships.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Battleships
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {

        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                context.Result = HandleBadRequestExceptions(context.Exception);
                context.ExceptionHandled = true;
            }

        }


        private IActionResult HandleBadRequestExceptions(Exception exception)
        {
            switch (exception)
            {
                case InvalidRequestException e:
                    return new BadRequestObjectResult(new ErrorResult(exception));
                default:
                    return null;
            }
        }
    }
}
