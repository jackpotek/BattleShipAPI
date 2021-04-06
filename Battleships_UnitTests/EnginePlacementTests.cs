
using Battleships.Configuration;
using Battleships.Controllers;
using Battleships.Engine;
using Battleships.Errors;
using Battleships.Models;
using Battleships.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Battleships_UnitTests
{

    public class EnginePlacementTests
    {


        private bool compareObjectJsons(object obj1, object obj2)
        {
            return JsonConvert.SerializeObject(obj1, Formatting.None).Equals(JsonConvert.SerializeObject(obj2, Formatting.None));
        }

        private BattleshipConfiguration prepareConfig()
        {
            var configShips = new List<ShipAmount> {
                new ShipAmount{ Size = 5, Amount = 1 },
                new ShipAmount{ Size = 4, Amount = 2 },
                new ShipAmount{ Size = 3, Amount = 3 },
                new ShipAmount{ Size = 2, Amount = 4 }
            };
            return new BattleshipConfiguration() { MatrixHeight = 10, MatrixWidth = 10, Ships = configShips };
        }

        private List<ShipPlacement> prepareValidShipsWithoutOne2Fielded()
        {
            List<ShipPlacement> correctShipsWithoutOne2Fielded = new List<ShipPlacement> {
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(1, 1), new Coordinate(1, 2) } },
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(2, 1), new Coordinate(2, 2) } },
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(3, 1), new Coordinate(3, 2) } },

                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(4, 1), new Coordinate(4, 2), new Coordinate(4, 3) } },
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(5, 1), new Coordinate(5, 2), new Coordinate(5, 3) } },
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(6, 1), new Coordinate(6, 2), new Coordinate(6, 3) } },

                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(7, 1), new Coordinate(7, 2), new Coordinate(7, 3), new Coordinate(7, 4) } },
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(8, 1), new Coordinate(8, 2), new Coordinate(8, 3), new Coordinate(8, 4) } },

                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(9, 1), new Coordinate(9, 2), new Coordinate(9, 3), new Coordinate(9, 4), new Coordinate(9, 5) } }
            };

            return correctShipsWithoutOne2Fielded;
        }

        private List<Ship> prepareTwoShipsWithOneDamage()
        {
            List<Ship> ships = new List<Ship>
            {
                new Ship { Placement = new ShipPlacement { Coordinates = new List<Coordinate>{ new Coordinate(1,1), new Coordinate(1,2)} } , Damages = new ShipPlacement { Coordinates = new List<Coordinate>{ new Coordinate(1,1)} }},
                new Ship { Placement = new ShipPlacement { Coordinates = new List<Coordinate>{ new Coordinate(2,1), new Coordinate(2,2)} } , Damages = new ShipPlacement { Coordinates = new List<Coordinate>() } } 
            };
            
            return ships;
        }

        private List<Ship> prepareTwoShipsWithOneMissingDamage()
        {
            List<Ship> ships = new List<Ship>
            {
                new Ship { Placement = new ShipPlacement { Coordinates = new List<Coordinate>{ new Coordinate(1,1), new Coordinate(1,2)} } , Damages = new ShipPlacement { Coordinates = new List<Coordinate>{ new Coordinate(1,1), new Coordinate(1, 2) } }},
                new Ship { Placement = new ShipPlacement { Coordinates = new List<Coordinate>{ new Coordinate(2,1), new Coordinate(2,2)} } , Damages = new ShipPlacement { Coordinates = new List<Coordinate>{ new Coordinate(2,1) } }}
            };

            return ships;
        }

        public static class MockMemoryCacheService
        {
            public static IMemoryCache GetMemoryCache(object expectedValue)
            {
                var mockMemoryCache = new Mock<IMemoryCache>();
                mockMemoryCache
                    .Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedValue))
                    .Returns(true);
                return mockMemoryCache.Object;
            }
        }

        public static class MockMemoryCacheServiceWithGetAndSet
        {
            public static IMemoryCache GetMemoryCache(object expectedValue)
            {
                var mockMemoryCache = new Mock<IMemoryCache>();
                mockMemoryCache
                    .Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedValue))
                    .Returns(true);

                mockMemoryCache.Setup
                    (cache =>
                        cache.CreateEntry(
                        It.IsAny<object>()
                        ))
                        .Returns(Mock.Of<ICacheEntry>());
                return mockMemoryCache.Object;
            }
        }

        //CREATE GAME

        [Fact]
        public async Task CreateNewGameTest()
        {
            var mockRandom = new Moq.Mock<Random>();
            mockRandom.Setup(rand => rand.Next(1, 10000)).Returns(() => 1234); //Random Number Generator returns 1234
            var randomGenerator = new RandomGenerator(mockRandom.Object);

            var mockCache = new Mock<IMemoryCache>();
            mockCache.Setup
                (cache =>
                    cache.CreateEntry(
                    It.IsAny<object>()
                    ))
                    .Returns(Mock.Of<ICacheEntry>());
            var mockConfig = new Mock<IOptions<BattleshipConfiguration>>();
            var mockEngine = new Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache.Object, mockConfig.Object, mockEngine.Object, randomGenerator);

            var result = controller.GetNewGame();

            var viewResult = Assert.IsType<ActionResult<GameStateResponse>>(result);
            Assert.True(compareObjectJsons(new GameStateResponse(1234), result.Value));
        }


        //PLACING SHIPS

        [Fact]
        public async Task InvalidGamePlacingTest()
        {
            object expected = null; //game not found
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 1 };

            var ex = Assert.Throws<InvalidGameIdException>(() => controller.PlaceShips(request));
            Assert.Equal("Invalid game Id 1234, no valid game with the given id exists.", ex.Message);
        }

        [Fact]
        public async Task ShootingGamePlacingTest()
        {
            object expected = new GameStateResponse(1234) { GameStatus = GameStatuses.SHOOTING };
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 1 };

            var ex = Assert.Throws<InvalidGameStatusPlacingException>(() => controller.PlaceShips(request));
            Assert.Equal("Game already started thus it's no longer possible to place ships, current mode: SHOOTING", ex.Message);
        }

        [Fact]
        public async Task FinishedGamePlacingTest()
        {
            object expected = new GameStateResponse(1234) { GameStatus = GameStatuses.FINISHED };
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 1 };

            var ex = Assert.Throws<InvalidGameStatusPlacingException>(() => controller.PlaceShips(request));
            Assert.Equal("Game already started thus it's no longer possible to place ships, current mode: FINISHED", ex.Message);
        }

        [Fact]
        public async Task InvalidPlayerIdPlacingTest()
        {
            object expected = new GameStateResponse(1234); //status = placing
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 15 };

            var ex = Assert.Throws<InvalidPlayerIdException>(() => controller.PlaceShips(request));
            Assert.Equal("Invalid Player Id. Only Players with Id 1 and 2 are permited", ex.Message);
        }

        [Fact]
        public async Task NoShipsPlacingTest()
        {
            object expected = new GameStateResponse(1234); //status = placing
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 1 };

            var ex = Assert.Throws<NoShipsProvidedException>(() => controller.PlaceShips(request));
            Assert.Equal("No ships provided in placement", ex.Message);
        }

        [Fact]
        public async Task CollidingShipsPlacingTest()
        {
            object expected = new GameStateResponse(1234); //status = placing
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            List<ShipPlacement> incorrectShips = new List<ShipPlacement> {  // COLLISION!
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(1, 1), new Coordinate(1, 2) } },
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(1, 2), new Coordinate(2, 2) } }
            };
            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 1, Ships = incorrectShips };

            var ex = Assert.Throws<CollidingShipsLogError>(() => controller.PlaceShips(request));
            Assert.Equal("Provided ships are colliding as they share coordinates", ex.Message);
        }

        [Fact]
        public async Task IncorrectCoordinatesShipsPlacingTest()
        {
            object expected = new GameStateResponse(1234); //status = placing
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            List<ShipPlacement> incorrectShips = new List<ShipPlacement> {  // INCORRECT COORDINATE 1,12
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(1, 12), new Coordinate(1, 2) } },
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(3, 2), new Coordinate(2, 2) } }
            };
            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 1, Ships = incorrectShips };

            var ex = Assert.Throws<InvalidCoordinatesException>(() => controller.PlaceShips(request));
            Assert.Equal("Invalid coordinates supplied, possible values: columns 1-10, rows 1-10", ex.Message);
        }

        [Fact]
        public async Task InvalidShipAmountPlacingTest()
        {
            object expected = new GameStateResponse(1234); //status = placing
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            List<ShipPlacement> incorrectShips = new List<ShipPlacement> { //ONLY 2 SHIPS
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(1, 1), new Coordinate(1, 2) } },
                new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(3, 2), new Coordinate(2, 2) } }
            };
            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 1, Ships = incorrectShips };

            var ex = Assert.Throws<InvalidShipAmountException>(() => controller.PlaceShips(request));
            Assert.Equal("Provided ships do not conform to the configured amounts, which is: [{\"Size\":5,\"Amount\":1},{\"Size\":4,\"Amount\":2},{\"Size\":3,\"Amount\":3},{\"Size\":2,\"Amount\":4}]", ex.Message);
        }

        [Fact]
        public async Task InvalidShipShapesPlacingTest()
        {
            object expected = new GameStateResponse(1234); //status = placing
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            List<ShipPlacement> incorrectShips = prepareValidShipsWithoutOne2Fielded();
            incorrectShips.Add(new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(8, 8), new Coordinate(9, 9) } }); //45 degree ship

            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 1, Ships = incorrectShips };

            var ex = Assert.Throws<InvalidShipShapesException>(() => controller.PlaceShips(request));
            Assert.Equal("All ships need to be on the vertical or horizontal line", ex.Message);
        }

        [Fact]
        public async Task InvalidShipIntegrityPlacingTest()
        {
            object expected = new GameStateResponse(1234); //status = placing
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            List<ShipPlacement> incorrectShips = prepareValidShipsWithoutOne2Fielded();
            incorrectShips.Add(new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(8, 8), new Coordinate(10, 8) } }); //desintegrated ship

            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 1, Ships = incorrectShips };

            var ex = Assert.Throws<InvalidShipIntegrityException>(() => controller.PlaceShips(request));
            Assert.Equal("Each ship needs to have connected coordinates", ex.Message);
        }

        [Fact]
        public async Task ProperPlacementWaitingForSecondPlayerPlacingTest()
        {
            object expected = new GameStateResponse(1234); //status = placing
            var mockCache = MockMemoryCacheServiceWithGetAndSet.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            List<ShipPlacement> correctShips = prepareValidShipsWithoutOne2Fielded();
            correctShips.Add(new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(8, 8), new Coordinate(9, 8) } });

            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 1, Ships = correctShips };

            var result = controller.PlaceShips(request);
            var viewResult = Assert.IsType<ActionResult<GameStateResponse>>(result);
            GameStateResponse expectedNewGame = new GameStateResponse(1234);

            Assert.Equal(result.Value.NextPlayer, 0); //still waiting for second player to place ships
            Assert.Equal(result.Value.GameStatus, GameStatuses.PLACING); //still waiting for second player to place ships
            Assert.True(result.Value.Player1Ships.Count > 0); //ships placed by first player
        }

        [Fact]
        public async Task ProperPlacementBothPlayersPlacingTest()
        {
            object expected = new GameStateResponse(1234) { Player1Ships = new List<Ship> { new Ship() } } ; //status = placing, Player 1 has placed their ships
            var mockCache = MockMemoryCacheServiceWithGetAndSet.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            List<ShipPlacement> correctShips = prepareValidShipsWithoutOne2Fielded();
            correctShips.Add(new ShipPlacement { Coordinates = new List<Coordinate> { new Coordinate(8, 8), new Coordinate(9, 8) } });

            ShipPlacementRequest request = new ShipPlacementRequest() { GameId = 1234, PlayerId = 2, Ships = correctShips };

            var result = controller.PlaceShips(request);
            var viewResult = Assert.IsType<ActionResult<GameStateResponse>>(result);

            Assert.Equal(result.Value.NextPlayer, 1); //shooting starts, player 1 is first to fire
            Assert.Equal(result.Value.GameStatus, GameStatuses.SHOOTING); //still waiting for second player to place ships
            Assert.True(result.Value.Player1Ships.Count > 0); //ships placed by first player
            Assert.True(result.Value.Player2Ships.Count > 0); //ships placed by first player
        }

        //SHOOTING

        [Fact]
        public async Task InvalidGameShotTest()
        {
            object expected = null; //game not found
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            ShotRequest request = new ShotRequest() { GameId = 1234, PlayerId = 1, Coordinate = new Coordinate(1, 1) };

            var ex = Assert.Throws<InvalidGameIdException>(() => controller.Shot(request));
            Assert.Equal("Invalid game Id 1234, no valid game with the given id exists.", ex.Message);
        }

        [Fact]
        public async Task PlacingGameShotTest()
        {
            object expected = new GameStateResponse(1234); //status = placing
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            ShotRequest request = new ShotRequest() { GameId = 1234, PlayerId = 1, Coordinate = new Coordinate(1, 1) };

            var ex = Assert.Throws<InvalidGameStatusShootingException>(() => controller.Shot(request));
            Assert.Equal("Game is not in the Shooting mode, current mode: PLACING", ex.Message);
        }

        [Fact]
        public async Task FinishedGameShotTest()
        {
            object expected = new GameStateResponse(1234) { GameStatus = GameStatuses.FINISHED };
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            ShotRequest request = new ShotRequest() { GameId = 1234, PlayerId = 1, Coordinate = new Coordinate(1, 1) };

            var ex = Assert.Throws<InvalidGameStatusShootingException>(() => controller.Shot(request));
            Assert.Equal("Game is not in the Shooting mode, current mode: FINISHED", ex.Message);
        }

        [Fact]
        public async Task IncorrectPlayerGameShotTest()
        {
            object expected = new GameStateResponse(1234) { GameStatus = GameStatuses.SHOOTING, NextPlayer = 1 };
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            ShotRequest request = new ShotRequest() { GameId = 1234, PlayerId = 2, Coordinate = new Coordinate(1, 1) };

            var ex = Assert.Throws<InvalidTurnPlayerException>(() => controller.Shot(request));
            Assert.Equal("Not your turn! Wait for player 1 to shot next", ex.Message);
        }

        [Fact]
        public async Task IncorrectCoordinatesShotTest()
        {
            object expected = new GameStateResponse(1234) { GameStatus = GameStatuses.SHOOTING, NextPlayer = 1 };
            var mockCache = MockMemoryCacheService.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);

            List<Coordinate> incorrectCoordinates = new List<Coordinate> {
                new Coordinate(-1,1),
                new Coordinate(1,-2),
                new Coordinate(0,1),
                new Coordinate(1,0),
                new Coordinate(15,5),
                new Coordinate(5,15),
                new Coordinate(15,15)};

            foreach(Coordinate c in  incorrectCoordinates){
                ShotRequest request = new ShotRequest() { GameId = 1234, PlayerId = 1, Coordinate = c };

                var ex = Assert.Throws<InvalidCoordinatesException>(() => controller.Shot(request));
                Assert.Equal("Invalid coordinates supplied, possible values: columns 1-10, rows 1-10", ex.Message);
            }
        }

        [Fact]
        public async Task ShootingAlreadyShotCoordinateShotTest()
        {
            object expected = new GameStateResponse(1234) { GameStatus = GameStatuses.SHOOTING, NextPlayer = 1, Player2Ships = prepareTwoShipsWithOneDamage() };
            var mockCache = MockMemoryCacheServiceWithGetAndSet.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);


            ShotRequest request = new ShotRequest() { GameId = 1234, PlayerId = 1, Coordinate = new Coordinate(1, 1) };

            var result = controller.Shot(request);
            var viewResult = Assert.IsType<ActionResult<GameStateResponse>>(result);

            Assert.Equal(result.Value.NextPlayer, 2); //swapped players
            Assert.Equal(result.Value.GameStatus, GameStatuses.SHOOTING); //game still going
            Assert.Equal(result.Value.Message, "Do not shoot the same target!"); //message
        }

        [Fact]
        public async Task ProperNonEndingShotShipNotDestroyedTest()
        {
            object expected = new GameStateResponse(1234) { GameStatus = GameStatuses.SHOOTING, NextPlayer = 1, Player2Ships = prepareTwoShipsWithOneDamage() };
            var mockCache = MockMemoryCacheServiceWithGetAndSet.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);


            ShotRequest request = new ShotRequest() { GameId = 1234, PlayerId = 1, Coordinate = new Coordinate(2,2) };

            var result = controller.Shot(request);
            var viewResult = Assert.IsType<ActionResult<GameStateResponse>>(result);

            Assert.Equal(result.Value.NextPlayer, 2); //swapped players
            Assert.Equal(result.Value.GameStatus, GameStatuses.SHOOTING); //game still going
            Assert.Equal(result.Value.Message, "Shot succeeded, Ship not yet destroyed"); //message
        }

        [Fact]
        public async Task ProperNonEndingShotShipDestroyedTest()
        {
            object expected = new GameStateResponse(1234) { GameStatus = GameStatuses.SHOOTING, NextPlayer = 1, Player2Ships = prepareTwoShipsWithOneDamage() };
            var mockCache = MockMemoryCacheServiceWithGetAndSet.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);


            ShotRequest request = new ShotRequest() { GameId = 1234, PlayerId = 1, Coordinate = new Coordinate(1, 2) };

            var result = controller.Shot(request);
            var viewResult = Assert.IsType<ActionResult<GameStateResponse>>(result);

            Assert.Equal(result.Value.NextPlayer, 2); //swapped players
            Assert.Equal(result.Value.GameStatus, GameStatuses.SHOOTING); //game still going
            Assert.Equal(result.Value.Message, "Shot succeeded, Ship destroyed"); //message
        }

        [Fact]
        public async Task MissedShotTest()
        {
            object expected = new GameStateResponse(1234) { GameStatus = GameStatuses.SHOOTING, NextPlayer = 1, Player2Ships = prepareTwoShipsWithOneDamage() };
            var mockCache = MockMemoryCacheServiceWithGetAndSet.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);


            ShotRequest request = new ShotRequest() { GameId = 1234, PlayerId = 1, Coordinate = new Coordinate(5, 5) };

            var result = controller.Shot(request);
            var viewResult = Assert.IsType<ActionResult<GameStateResponse>>(result);

            Assert.Equal(result.Value.NextPlayer, 2); //swapped players
            Assert.Equal(result.Value.GameStatus, GameStatuses.SHOOTING); //game still going
            Assert.Equal(result.Value.Message, "Missed!"); //message
        }

        [Fact]
        public async Task ProperEndingShotShipDestroyedGameEndTest()
        {
            object expected = new GameStateResponse(1234) { GameStatus = GameStatuses.SHOOTING, NextPlayer = 1, Player2Ships = prepareTwoShipsWithOneMissingDamage() };
            var mockCache = MockMemoryCacheServiceWithGetAndSet.GetMemoryCache(expected);

            var mockConfig = new Moq.Mock<IOptions<BattleshipConfiguration>>();
            mockConfig.Setup(config => config.Value).Returns(() => prepareConfig());
            var mockEngine = new Moq.Mock<GameEngine>(mockConfig.Object);
            var controller = new BattleShipController(mockCache, mockConfig.Object, mockEngine.Object, null);


            ShotRequest request = new ShotRequest() { GameId = 1234, PlayerId = 1, Coordinate = new Coordinate(2, 2) };

            var result = controller.Shot(request);
            var viewResult = Assert.IsType<ActionResult<GameStateResponse>>(result);

            Assert.Equal(result.Value.NextPlayer, 0); //no more moves
            Assert.Equal(result.Value.GameStatus, GameStatuses.FINISHED); //Game finished
            Assert.Equal(result.Value.WinnerPlayer, 1); //winner
            Assert.Equal(result.Value.Message, "Game finished, player 1 has won the game"); //message
        }

    }
}
