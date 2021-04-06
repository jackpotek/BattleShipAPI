namespace Battleships.Errors
{
    public class InvalidGameStatusShootingException : InvalidRequestException
    {
        public InvalidGameStatusShootingException(string gameStatus)
            : base($"Game is not in the Shooting mode, current mode: {gameStatus}")
        {
        }
    }
}
