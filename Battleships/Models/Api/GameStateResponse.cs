using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Battleships.Models.Api
{
    public class GameStateResponse
    {
        public int GameId { get; set; }
        public String GameStatus { get; set; }
        [JsonIgnore]
        public List<Ship> Player1Ships { get; set; }
        [JsonIgnore]
        public List<Ship> Player2Ships { get; set; }
        public int NextPlayer { get; set; }
        public int WinnerPlayer { get; set; }
        public string Message { get; set; }

        public GameStateResponse(int gameId)
        {
            GameId = gameId;
            GameStatus = GameStatuses.PLACING;
            Player1Ships = new List<Ship>();
            Player2Ships = new List<Ship>();
            NextPlayer = 0;
            WinnerPlayer = 0;
        }

    }
}
