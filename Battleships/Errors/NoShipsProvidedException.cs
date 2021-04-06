namespace Battleships.Errors
{
    public class NoShipsProvidedException : InvalidRequestException
    {
        public NoShipsProvidedException()
            : base($"No ships provided in placement")
        {
        }
    }
}
