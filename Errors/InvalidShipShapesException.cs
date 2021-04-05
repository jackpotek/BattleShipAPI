using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Errors
{
    internal class InvalidShipShapesException : InvalidRequestException
    {
        public InvalidShipShapesException()
            : base($"All ships need to be on the vertical or horizontal line")
        {
        }
    }
}
