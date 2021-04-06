using Battleships.Configuration;
using Battleships.Engine;
using Battleships.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;

namespace Battleships.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BattleShipController : ControllerBase
    {

        private readonly IMemoryCache _cache;
        private readonly IOptions<BattleshipConfiguration> _config;
        private readonly GameEngine _engine;
        private readonly RandomGenerator _randomGenerator;

        public BattleShipController(IMemoryCache memoryCache, IOptions<BattleshipConfiguration> config, GameEngine engine, RandomGenerator randomGenerator)
        {
            _cache = memoryCache;
            _config = config;
            _engine = engine;
            _randomGenerator = randomGenerator;
        }

        [HttpGet]
        [Route("NewGame")]
        public ActionResult<GameStateResponse> GetNewGame()
        {
            int random = _randomGenerator.GetRandomNumber(10000);
            while (_cache.Get(random) != null)
            {
                random = _randomGenerator.GetRandomNumber(10000);
            }
            var entry = _cache.Set(random, new GameStateResponse(random), new MemoryCacheEntryOptions().SetSize(1).SetSlidingExpiration(TimeSpan.FromDays(10)));
            return entry;
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
