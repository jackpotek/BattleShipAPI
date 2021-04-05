using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Errors
{
    internal class InvalidGameIdException : InvalidRequestException
    {
        public InvalidGameIdException(int? gameId)
            : base($"Invalid game Id {gameId}, no valid game with the given id exists.")
        {
        }
    }
}
