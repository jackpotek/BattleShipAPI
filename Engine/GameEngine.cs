using Battleships.Configuration;
using Battleships.Errors;
using Battleships.Models;
using Battleships.Models.Api;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Battleships.Engine
{
    public class GameEngine
    {
        private readonly IOptions<BattleshipConfiguration> _config;


        public GameEngine(IOptions<BattleshipConfiguration> config)
        {
            _config = config;
        }

        public void PlaceShips(ShipPlacementRequest request, GameStateResponse gameState)
        {
            if (gameState == null)
                throw new InvalidGameIdException(request.GameId);

            if (gameState.GameStatus != GameStatuses.PLACING)
                throw new InvalidGameStatusPlacingException(gameState.GameStatus);

            if (request.PlayerId != 1 && request.PlayerId != 2)
                throw new InvalidPlayerIdException();

            validateShipPlacement(request);
            performPlacement(request, gameState);
        }

        public void Shot(ShotRequest request, GameStateResponse gameState)
        {
            if (gameState == null)
                throw new InvalidGameIdException(request.GameId);

            if (gameState.GameStatus != GameStatuses.SHOOTING)
                throw new InvalidGameStatusShootingException(gameState.GameStatus);

            if (gameState.NextPlayer != request.PlayerId)
                throw new InvalidTurnPlayerException(gameState.NextPlayer);

            if (request.Coordinate.Row < 1 || request.Coordinate.Row > _config.Value.MatrixHeight || request.Coordinate.Column < 1 || request.Coordinate.Column > _config.Value.MatrixWidth)
                throw new InvalidCoordinatesException(_config.Value.MatrixWidth, _config.Value.MatrixHeight);

            performShot(gameState, request.Coordinate);
        }

        private void validateShipPlacement(ShipPlacementRequest request)
        {
            int width = _config.Value.MatrixWidth;
            int height = _config.Value.MatrixHeight;
            List<ShipAmount> shipsSetup = _config.Value.Ships;

            if (request.Ships == null || request.Ships.Count() == 0)
                throw new NoShipsProvidedException();

            //no collisions
            if (request.Ships.SelectMany(i => i.Coordinates).Count() != request.Ships.SelectMany(i => i.Coordinates).Distinct().Count())
                throw new CollidingShipsLogError();

            if (request.Ships.SelectMany(i => i.Coordinates).Where(i => i.Row < 1 || i.Row > height || i.Column < 1 || i.Column > width).Count() > 0)
                throw new InvalidCoordinatesException(width, height);

            //one ship with 5 fields, two ships with 4 fields, three ships with 3 fields, four ships with 2 fields
            foreach (var variant in shipsSetup)
            {
                if (request.Ships.Where(i => i.Coordinates.Count() == variant.Size).Count() != variant.Amount)
                    throw new InvalidShipAmountException(JsonConvert.SerializeObject(shipsSetup, Formatting.None));
            }

            //ships should be on the same row or the same column and coordinates should be adjacent to each other
            foreach (ShipPlacement ship in request.Ships)
            {
                if (ship.Coordinates.Select(i => i.Column).Distinct().Count() != 1 && ship.Coordinates.Select(i => i.Row).Distinct().Count() != 1)
                    throw new InvalidShipShapesException();

                if (ship.Coordinates.Select(i => i.Column).Distinct().Count() == 1)
                {
                    if (ship.Coordinates.Select(i => i.Row).Max() - ship.Coordinates.Select(i => i.Row).Min() != ship.Coordinates.Count() - 1)
                        throw new InvalidShipIntegrityException();
                }
                if (ship.Coordinates.Select(i => i.Row).Distinct().Count() == 1)
                {
                    if (ship.Coordinates.Select(i => i.Column).Max() - ship.Coordinates.Select(i => i.Column).Min() != ship.Coordinates.Count() - 1)
                        throw new InvalidShipIntegrityException();
                }
            }
        }

        private void performPlacement(ShipPlacementRequest request, GameStateResponse gameState)
        {
            if (request.PlayerId == 1)
            {
                gameState.Player1Ships = request.Ships.Select(i => new Ship
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
            else if (request.PlayerId == 2)
            {
                gameState.Player2Ships = request.Ships.Select(i => new Ship
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

            if (gameState.Player1Ships.Count() > 0 && gameState.Player2Ships.Count() > 0)
            {
                gameState.GameStatus = GameStatuses.SHOOTING;
                gameState.NextPlayer = 1;
            }

        }



        private void performShot(GameStateResponse gameState, Coordinate coordinate)
        {
            gameState.Message = "";

            List<Ship> ships;
            if (gameState.NextPlayer == 1)
                ships = gameState.Player2Ships;
            else
                ships = gameState.Player1Ships;

            Ship shipBeeingShot = ships.Find(i => i.Placement.Coordinates.Where(j => j.Column == coordinate.Column && j.Row == coordinate.Row).Count() == 1);

            if (shipBeeingShot?.Damages.Coordinates.Where(i => i.Column == coordinate.Column && i.Row == coordinate.Row).Count() > 0)
            {
                gameState.Message = "Do not shoot the same target!";
                swapPlayers(gameState);
                return;
            }
            shipBeeingShot?.Damages.Coordinates.Add(coordinate);

            if (shipBeeingShot != null)
            {
                gameState.Message = "Shot succeeded";

                if (shipBeeingShot.checkDestroyed())
                    gameState.Message = "Shot succeeded, ship destroyed";

                if (ships.Where(i => i.checkDestroyed()).Count() == 10)
                {
                    gameState.WinnerPlayer = gameState.NextPlayer;
                    gameState.GameStatus = GameStatuses.FINISHED;
                    gameState.Message = "Game finished, player " + gameState.NextPlayer + " has won the game";
                    gameState.NextPlayer = 0;
                    return;
                }
            }
            else
            {
                gameState.Message = "Missed!";

            }
            swapPlayers(gameState);
        }

        private void swapPlayers(GameStateResponse gameState)
        {
            int next;
            if (gameState.NextPlayer == 1)
                next = 2;
            else
                next = 1;
            gameState.NextPlayer = next;
        }


    }
}
