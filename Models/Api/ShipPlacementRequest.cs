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

        public void validateShipPlacement(int width, int height, List<ShipAmount> shipsSetup)
        {

            if (Ships == null || Ships.Count() == 0)
                throw new NoShipsProvidedException();

            //no collisions
            if (Ships.SelectMany(i => i.Coordinates).Count() != Ships.SelectMany(i => i.Coordinates).Distinct().Count())
                throw new CollidingShipsLogError();

            if (Ships.SelectMany(i => i.Coordinates).Where(i => i.Row < 1 || i.Row > height || i.Column < 1 || i.Column > width).Count() > 0)
                throw new InvalidCoordinatesException(width, height);

            //one ship with 5 fields, two ships with 4 fields, three ships with 3 fields, four ships with 2 fields
            foreach(var variant in shipsSetup)
            {
                if (Ships.Where(i => i.Coordinates.Count() == variant.Size).Count() != variant.Amount)
                    throw new InvalidShipAmountException(JsonConvert.SerializeObject(shipsSetup, Formatting.None));
            }

            //ships should be on the same row or the same column and coordinates should be adjacent to each other
            foreach(ShipPlacement ship in Ships)
            {
                if (ship.Coordinates.Select(i => i.Column).Distinct().Count() != 1 && ship.Coordinates.Select(i => i.Row).Distinct().Count() != 1)
                    throw new InvalidShipShapesException();

                if (ship.Coordinates.Select(i => i.Column).Distinct().Count() == 1)
                {
                    if (ship.Coordinates.Select(i => i.Row).Max() - ship.Coordinates.Select(i => i.Row).Min() != ship.Coordinates.Count() - 1)
                        throw new InvalidShipIntegrityException();
                }
                if (ship.Coordinates.Select(i => i.Row).Distinct().Count() == 1)
                {
                    if (ship.Coordinates.Select(i => i.Column).Max() - ship.Coordinates.Select(i => i.Column).Min() != ship.Coordinates.Count() - 1)
                        throw new InvalidShipIntegrityException();
                }
            }
        }
    }
}
