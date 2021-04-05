using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Errors
{
    internal class InvalidCoordinatesException : InvalidRequestException
    {
        public InvalidCoordinatesException(int? width, int? height)
            : base($"Invalid coordinates supplied, possible values: columns 1-{width}, rows 1-{height}")
        {
        }
    }
}
