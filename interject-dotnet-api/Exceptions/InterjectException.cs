using System;

namespace Interject.Exceptions
{
    public class InterjectException : Exception
    {
        public int StatusCode { get; set; }

        public InterjectException() { }

        public InterjectException(string message) : base(message)
        {
            StatusCode = 400;
        }

        public InterjectException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public InterjectException(string message, Exception exception) : base(message, exception) { }
    }
}