using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        public void performShot(Coordinate coordinate)
        {
            Message = "";

            List<Ship> ships;
            if (NextPlayer == 1)
                ships = Player2Ships;
            else
                ships = Player1Ships;

            Ship shipBeeingShot = ships.Find(i => i.Placement.Coordinates.Where(j=>j.Column == coordinate.Column && j.Row == coordinate.Row).Count() == 1);

            if (shipBeeingShot?.Damages.Coordinates.Where(i => i.Column == coordinate.Column && i.Row == coordinate.Row).Count() > 0)
            {
                Message = "Do not shoot the same target!";
                swapPlayers();
                return;
            }
            shipBeeingShot?.Damages.Coordinates.Add(coordinate);

            if (shipBeeingShot != null)
            {
                Message = "Shot succeeded";

                if(shipBeeingShot.checkDestroyed())
                    Message = "Shot succeeded, ship destroyed";

                if (ships.Where(i=>i.checkDestroyed()).Count() == 10)
                {
                    WinnerPlayer = NextPlayer;
                    GameStatus = GameStatuses.FINISHED;
                    Message = "Game finished, player " + NextPlayer + " has won the game";
                    NextPlayer = 0;
                    return;
                }
            }
            else
            {
                Message = "Missed!";
                
            }
            swapPlayers();
        }

        public void performPlacement(int playerId, List<ShipPlacement> shipPlacements)
        {
            if (playerId == 1)
            {
                Player1Ships = shipPlacements.Select(i => new Ship
                {
                    Placement = new ShipPlacement
                    {
                        Coordinates = i.Coordinates
                    },
                    Damages = new ShipPlacement
                    {
                        Coordinates = new List<Coordinate>()
                    }

                }).ToList();
            }
            else if (playerId == 2)
            {
                Player2Ships = shipPlacements.Select(i => new Ship
                {
                    Placement = new ShipPlacement
                    {
                        Coordinates = i.Coordinates
                    },
                    Damages = new ShipPlacement
                    {
                        Coordinates = new List<Coordinate>()
                    }
                }).ToList();
            }
            
            if (Player1Ships.Count() > 0 && Player2Ships.Count() > 0)
            {
                GameStatus = GameStatuses.SHOOTING;
                NextPlayer = 1;
            }

        }

        private void swapPlayers()
        {
            int next;
            if (NextPlayer == 1)
                next = 2;
            else
                next = 1;
            NextPlayer = next;
        }

    }
}
