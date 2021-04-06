namespace Battleships.Models.Api
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
