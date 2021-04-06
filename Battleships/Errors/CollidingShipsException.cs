namespace Battleships.Errors
{
    public class CollidingShipsLogError : InvalidRequestException
    {
        public CollidingShipsLogError()
            : base($"Provided ships are colliding as they share coordinates")
        {
        }
    }
}
