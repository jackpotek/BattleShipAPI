using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Battleships.Models;
using Microsoft.Extensions.Options;
using Battleships.Configuration;
using Battleships.Models.Api;
using Battleships.Errors;
using Battleships.Engine;

namespace Battleships.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BattleShipController : ControllerBase
    {

        private readonly IMemoryCache _cache;
        private readonly ILogger<BattleShipController> _logger;
        private readonly IOptions<BattleshipConfiguration> _config;
        private readonly GameEngine _engine;

        public BattleShipController(ILogger<BattleShipController> logger, IMemoryCache memoryCache, IOptions<BattleshipConfiguration> config, GameEngine engine)
        {
            _cache = memoryCache;
            _logger = logger;
            _config = config;
            _engine = engine;
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
            _engine.Shot(request, game);
            _cache.Set(game.GameId, game);
            return game;
        }

        [HttpPost]
        [Route("PlaceShips")]
        public ActionResult<GameStateResponse> PlaceShips(ShipPlacementRequest request)
        {
            _cache.TryGetValue(request.GameId, out GameStateResponse game);
            _engine.PlaceShips(request, game);
            _cache.Set(game.GameId, game);
            return game;
        }

    }
}
