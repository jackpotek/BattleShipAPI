using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Models
{
    public class ShipPlacementRequest
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }

        public List<ShipPlacement> Ships { get; set; }

        public string validateShipPlacement()
        {

            if (Ships == null || Ships.Count() == 0)
                return "No ships provided";

            //no collisions
            if (Ships.SelectMany(i => i.Coordinates).Count() != Ships.SelectMany(i => i.Coordinates).Distinct().Count())
                return "Non-unique ship coordinates provided";

            if (Ships.SelectMany(i => i.Coordinates).Where(i => i.Row < 1 || i.Row > 10 || i.Column < 1 || i.Column > 10).Count() > 0)
            {
                return "Provided coordinates outside of 10 x 10 matrix - numbering from 1-10";
            }

            //one ship with 5 fields, two ships with 4 fields, three ships with 3 fields, four ships with 2 fields
            var variants = new[]{ new { ships = 1, fields = 5 }, new { ships = 2, fields = 4 }, new { ships = 3, fields = 3 }, new { ships = 4, fields = 2 } }.ToList();
            foreach(var variant in variants)
            {
                if (Ships.Where(i => i.Coordinates.Count() == variant.fields).Count() != variant.ships)
                    return "There should be one ship with 5 fields, two ships with 4 fields, three ships with 3 fields, four ships with 2 fields";
            }

            //ships should be on the same row or the same column and coordinates should be adjacent to each other
            foreach(ShipPlacement ship in Ships)
            {
                if (ship.Coordinates.Select(i => i.Column).Distinct().Count() != 1 && ship.Coordinates.Select(i => i.Row).Distinct().Count() != 1)
                    return "All ships need to be on the vertical or horizontal line";

                if (ship.Coordinates.Select(i => i.Column).Distinct().Count() == 1)
                {
                    if (ship.Coordinates.Select(i => i.Row).Max() - ship.Coordinates.Select(i => i.Row).Min() != ship.Coordinates.Count() - 1)
                        return "Each ship needs to have connected coordinates";
                }
                if (ship.Coordinates.Select(i => i.Row).Distinct().Count() == 1)
                {
                    if (ship.Coordinates.Select(i => i.Column).Max() - ship.Coordinates.Select(i => i.Column).Min() != ship.Coordinates.Count() - 1)
                        return "Each ship needs to have connected coordinates";
                }
            }

            return "";
            
        }

    }
}
