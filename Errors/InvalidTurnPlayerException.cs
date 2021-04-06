namespace Battleships.Errors
{
    internal class InvalidTurnPlayerException : InvalidRequestException
    {
        public InvalidTurnPlayerException(int? playerId)
            : base($"Not your turn! Wait for player {playerId} to shot next")
        {
        }
    }
}
