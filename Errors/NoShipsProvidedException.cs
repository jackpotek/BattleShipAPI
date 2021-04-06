namespace Battleships.Errors
{
    internal class NoShipsProvidedException : InvalidRequestException
    {
        public NoShipsProvidedException()
            : base($"No ships provided in placement")
        {
        }
    }
}
