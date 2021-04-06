using System;

namespace Battleships.Errors
{
    public class InvalidRequestException : Exception
    {

        protected InvalidRequestException(string message) : base(message)
        {
        }
    }
}
