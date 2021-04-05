using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Errors
{
    internal class InvalidShipAmountException : InvalidRequestException
    {
        public InvalidShipAmountException(string jsonShipConfig)
            : base($"Provided ships do not conform to the configured amounts, which is: {jsonShipConfig}")
        {
        }
    }
}
