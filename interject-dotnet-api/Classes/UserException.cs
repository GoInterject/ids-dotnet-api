using System;

namespace Interject
{
    public class UserException : Exception
    {
        public int StatusCode { get; set; }

        public UserException() { }

        public UserException(string message) : base(message)
        {
            StatusCode = 400;
        }

        public UserException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}