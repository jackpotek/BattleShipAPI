using System.Collections.Generic;

namespace Battleships.Configuration
{
    public class BattleshipConfiguration
    {
        public int MatrixWidth { get; set; }
        public int MatrixHeight { get; set; }
        public List<ShipAmount> Ships { get; set; }
    }
}
