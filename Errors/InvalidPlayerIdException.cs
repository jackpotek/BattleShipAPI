using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Errors
{
    internal class InvalidPlayerIdException : InvalidRequestException
    {
        public InvalidPlayerIdException()
            : base($"Invalid Player Id. Only Players with Id 1 and 2 are permited")
        {
        }
    }
}
