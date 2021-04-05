using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Errors
{
    internal class InvalidShipIntegrityException : InvalidRequestException
    {
        public InvalidShipIntegrityException()
            : base($"Each ship needs to have connected coordinates")
        {
        }
    }
}
