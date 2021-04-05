using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Errors
{
    internal class NoShipsProvidedException : InvalidRequestException
    {
        public NoShipsProvidedException()
            : base($"No ships provided in placement")
        {
        }
    }
}
