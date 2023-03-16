using Interject.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Interject
{
    /// <summary>
    /// Wraps the request in the request pipeline and intercepts errors.<br/>
    /// All responses are returned as standard <see cref="InterjectResponse"/>
    /// objects. <see cref="UserException"/> will pass the message as the
    /// <see cref="InterjectResponse.ErrorMessage"/>. Any othe exceptions thrown
    /// are obfuscated by hard coding the message to a generic response.
    /// </summary>
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