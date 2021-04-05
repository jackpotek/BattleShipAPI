using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Errors
{
    internal class CollidingShipsLogError : InvalidRequestException
    {
        public CollidingShipsLogError()
            : base($"Provided ships are colliding as they share coordinates")
        {
        }
    }
}
