using Interject.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Interject.Exceptions
{
    /// <summary>
    /// Wraps the request in the request pipeline and intercepts errors.<br/>
    /// All responses are returned as standard <see cref="InterjectResponseDTO"/>
    /// objects. <see cref="InterjectException"/> will pass the message as the
    /// <see cref="InterjectResponseDTO.ErrorMessage"/>. Any othe exceptions thrown
    /// are obfuscated by hard coding the message to a generic response.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // // For debugging:
                // // This allows reading of the post body as a string from the
                // // reqCxt variable below.
                // var request = context.Request;
                // if (request.Method == HttpMethods.Post && request.ContentLength > 0)
                // {
                //     request.EnableBuffering();
                //     var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                //     await request.Body.ReadAsync(buffer, 0, buffer.Length);
                //     var reqCxt = Encoding.UTF8.GetString(buffer);

                //     // Make it a bit easier to read.
                //     reqCxt = reqCxt.Replace("\\r", "");
                //     reqCxt = reqCxt.Replace("\\n ", "");
                //     reqCxt = reqCxt.Replace("\\n<", "<");
                //     reqCxt = reqCxt.Replace("\\", "");

                //     request.Body.Position = 0;
                // }

                if (context.Request.Method == HttpMethods.Post && context.Request.ContentLength > 0)
                {
                    throw new InterjectException("testing 401", 401);
                }

                await _next(context);
            }
            catch (Exception error)
            {
                InterjectResponseDTO responseContent;

                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case InterjectException e:
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