using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Configuration
{
    public class BattleshipConfiguration
    {
        public int MatrixWidth { get; set; }
        public int MatrixHeight { get; set; }
        public List<ShipAmount> Ships { get; set; }
    }
}
