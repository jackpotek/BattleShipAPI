using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Errors
{
    public class InvalidRequestException : Exception
    {

        protected InvalidRequestException(string message): base(message)
        {
        }
    }
}
