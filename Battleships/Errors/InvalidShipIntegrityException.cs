namespace Battleships.Errors
{
    public class InvalidShipIntegrityException : InvalidRequestException
    {
        public InvalidShipIntegrityException()
            : base($"Each ship needs to have connected coordinates")
        {
        }
    }
}
