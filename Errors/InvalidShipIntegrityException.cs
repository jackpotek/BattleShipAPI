namespace Battleships.Errors
{
    internal class InvalidShipIntegrityException : InvalidRequestException
    {
        public InvalidShipIntegrityException()
            : base($"Each ship needs to have connected coordinates")
        {
        }
    }
}
