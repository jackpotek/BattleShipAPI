using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Battleships.Models;
using Microsoft.Extensions.Options;
using Battleships.Configuration;
using Battleships.Models.Api;
using Battleships.Errors;

namespace Battleships.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BattleShipController : ControllerBase
    {

        private readonly IMemoryCache _cache;
        private readonly ILogger<BattleShipController> _logger;
        private readonly IOptions<BattleshipConfiguration> _config;

        public BattleShipController(ILogger<BattleShipController> logger, IMemoryCache memoryCache, IOptions<BattleshipConfiguration> config)
        {
            _cache = memoryCache;
            _logger = logger;
            _config = config;
        }


        [Route("/error")]
        public IActionResult Error() => Problem();

        [HttpGet]
        [Route("NewGame")]
        public ActionResult<GameStateResponse> GetNewGame()
        {
            var rng = new Random();
            int random = rng.Next(1, 10000);
            while (_cache.Get(random) != null){
                random = rng.Next(1, 10000);
            }

            GameStateResponse game;

            using (var entry = _cache.CreateEntry(random))
            {
                game = new GameStateResponse(random);
                entry.Value = game;
                entry.AbsoluteExpiration = DateTime.UtcNow.AddDays(10);
            }

            return game;
        }

        [HttpPost]
        [Route("Shot")]
        public ActionResult<GameStateResponse> Shot(ShotRequest request)
        {
            _cache.TryGetValue(request.GameId, out GameStateResponse game);
            if (game == null)
                throw new InvalidGameIdException(request.GameId);

            if (game.GameStatus != GameStatuses.SHOOTING)
                throw new InvalidGameStatusShootingException(game.GameStatus);

            if (game.NextPlayer != request.PlayerId)
                throw new InvalidTurnPlayerException(game.NextPlayer);

            request.validateShot(_config.Value.MatrixWidth, _config.Value.MatrixHeight);
            game.performShot(request.Coordinate);
            _cache.Set(game.GameId, game);
            return game;

        }

        [HttpPost]
        [Route("PlaceShips")]
        public ActionResult<GameStateResponse> PlaceShips(ShipPlacementRequest request)
        {
            _cache.TryGetValue(request.GameId, out GameStateResponse game);
            if(game == null)
                throw new InvalidGameIdException(request.GameId);

            if (game.GameStatus != GameStatuses.PLACING)
                throw new InvalidGameStatusPlacingException(game.GameStatus);
            
            if(request.PlayerId != 1 && request.PlayerId != 2)
                throw new InvalidPlayerIdException();

            request.validateShipPlacement(_config.Value.MatrixWidth, _config.Value.MatrixHeight, _config.Value.Ships);
            
            game.performPlacement(request.PlayerId, request.Ships);

            _cache.Set(game.GameId, game);
            return game;
        }

    }
}
