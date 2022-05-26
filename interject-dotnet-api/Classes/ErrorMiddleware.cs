using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Interject.Classes;

namespace Interject
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                InterjectResponse responseContent;

                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case UserException e:
                        context.Response.StatusCode = e.StatusCode;
                        responseContent = new();
                        responseContent.ErrorMessage = e.Message;
                        break;
                    default:
                        context.Response.StatusCode = 500;
                        responseContent = new();
                        responseContent.ErrorMessage = "A system error occurred in the data api.";
                        break;
                }

                if (!context.Response.HasStarted)
                {
                    var result = JsonConvert.SerializeObject(responseContent);
                    await response.WriteAsync(result);
                }
            }
        }
    }
}