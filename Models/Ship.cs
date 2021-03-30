using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Models
{
    public class Ship
    {
        public ShipPlacement Placement { get; set; }
        public ShipPlacement Damages { get; set; }

        public bool checkDestroyed()
        {
            return Placement.Coordinates.Count == Damages.Coordinates.Count;
        }

    }
}
