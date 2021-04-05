using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
