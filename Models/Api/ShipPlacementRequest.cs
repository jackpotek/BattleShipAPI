using Battleships.Configuration;
using Battleships.Errors;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Battleships.Models.Api
{
    public class ShipPlacementRequest
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }

        public List<ShipPlacement> Ships { get; set; }

    }
}
