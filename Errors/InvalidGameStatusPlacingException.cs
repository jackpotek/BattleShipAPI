namespace Battleships.Errors
{
    internal class InvalidGameStatusPlacingException : InvalidRequestException
    {
        public InvalidGameStatusPlacingException(string gameStatus)
            : base($"Game already started thus it's no longer possible to place ships, current mode: {gameStatus}")
        {
        }
    }
}
