using System;

namespace Interject.DataApi.ApiExceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public ApiException() { }

        public ApiException(string message) : base(message)
        {
            StatusCode = 400;
        }

        public ApiException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public ApiException(string message, Exception exception) : base(message, exception) { }
    }
}