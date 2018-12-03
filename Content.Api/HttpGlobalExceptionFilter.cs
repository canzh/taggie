﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Content.Api
{
    public partial class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            //if (context.Exception.GetType() == typeof(BasketDomainException))
            //{
            //    var json = new
            //    {
            //        Messages = new[] { context.Exception.Message }
            //    };

            //    context.Result = new BadRequestObjectResult(json);
            //    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //}
            //else
            {
                var json = new
                {
                    Messages = new[] { "An error occurred. Try it again." }
                };

                //if (env.IsDevelopment())
                //{
                //    json.DeveloperMessage = context.Exception;
                //}

                //context.Result = new InternalServerErrorObjectResult(json);

                context.Result = new BadRequestObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            context.ExceptionHandled = true;
        }
    }
}
