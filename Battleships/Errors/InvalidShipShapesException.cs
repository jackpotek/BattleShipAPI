namespace Battleships.Errors
{
    public class InvalidShipShapesException : InvalidRequestException
    {
        public InvalidShipShapesException()
            : base($"All ships need to be on the vertical or horizontal line")
        {
        }
    }
}
