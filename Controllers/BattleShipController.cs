using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;
using Battleships.Models;

namespace Battleships.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BattleShipController : ControllerBase
    {

        private IMemoryCache _cache;


        [Route("/error")]
        public IActionResult Error() => Problem();

        [HttpGet]
        [Route("NewGame")]
        public ActionResult<GameState> GetNewGame()
        {
            var rng = new Random();
            int random = rng.Next(1, 10000);
            while (_cache.Get(random) != null){
                random = rng.Next(1, 10000);
            }

            GameState game;

            using (var entry = _cache.CreateEntry(random))
            {
                game = new GameState(random);
                entry.Value = game;
                entry.AbsoluteExpiration = DateTime.UtcNow.AddDays(10);
            }

            return game;
        }

        [HttpPost]
        [Route("Shot")]
        public ActionResult<GameState> Shoot(ShootRequest request)
        {
            _cache.TryGetValue(request.GameId, out GameState game);
            if (game == null)
                throw new Exception("Invalid GameId");

            if (game.GameStatus != GameStatuses.SHOOTING)
                throw new Exception("Game is not in the Shooting mode, current mode: " + game.GameStatus);

            if (game.NextPlayer != request.PlayerId)
                throw new Exception("Not your turn! Wait for player " + game.NextPlayer + " to shot next");

            string validationResult = request.validateShot();
            if (validationResult != "")
                throw new Exception(validationResult);

            game.performShot(request.Coordinate);
            _cache.Set(game.GameId, game);
            return game;

        }

        [HttpPost]
        [Route("PlaceShips")]
        public ActionResult<GameState> PlaceShips(ShipPlacementRequest request)
        {
            _cache.TryGetValue(request.GameId, out GameState game);
            if(game == null)
                throw new Exception("Invalid GameId");
            
            if(game.GameStatus != GameStatuses.PLACING)
                throw new Exception("Game already started");
            
            if(request.PlayerId != 1 && request.PlayerId != 2)
                throw new Exception("Only players with Id 1 and 2 are permitted");

            string validationResult = request.validateShipPlacement();
            if(validationResult != "")
                throw new Exception(validationResult);

            game.performPlacement(request.PlayerId, request.Ships);

            
            _cache.Set(game.GameId, game);
            return game;
        }



        private readonly ILogger<BattleShipController> _logger;

        public BattleShipController(ILogger<BattleShipController> logger, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _logger = logger;
        }

    }
}
