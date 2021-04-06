using System.Collections.Generic;

namespace Battleships.Models.Api
{
    public class ShipPlacementRequest
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }

        public List<ShipPlacement> Ships { get; set; }

    }
}
