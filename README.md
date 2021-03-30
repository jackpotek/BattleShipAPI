# Battleship game

This is a simple battleship game API

## Installation

Using Visual Studio load the solution and launch the Battleships profile

## Usage

There are three API methods:

```GET https://localhost:5001/battleship/newgame```

Generates new game (with a random unique `GameID` which will be used in other methods)

Sample response:

```{
    "GameId": 3637,
    "GameStatus": "PLACING",
    "NextPlayer": 0,
    "WinnerPlayer": 0,
    "Message": null
}
```

```POST https://localhost:5001/battleship/placeships```

Places all ships for a player on the 10x10 board. Once both players (1 and 2) place their ships, the game goes into `SHOOTING` mode.

Proper request (with 10 ships [make sure the gameId is correct]):

```
{
   "gameId":3637,
   "playerId":1,
   "ships":[
      {
         "coordinates":[
            {
               "row":1,
               "column":2
            },
            {
               "row":2,
               "column":2
            },
            {
               "row":3,
               "column":2
            },
            {
               "row":4,
               "column":2
            },
            {
               "row":5,
               "column":2
            }
         ]
      },
      {
         "coordinates":[
            {
               "row":1,
               "column":1
            },
            {
               "row":2,
               "column":1
            },
            {
               "row":3,
               "column":1
            },
            {
               "row":4,
               "column":1
            }
         ]
      },
      {
         "coordinates":[
            {
               "row":1,
               "column":3
            },
            {
               "row":2,
               "column":3
            },
            {
               "row":3,
               "column":3
            },
            {
               "row":4,
               "column":3
            }
         ]
      },
      {
         "coordinates":[
            {
               "row":1,
               "column":5
            },
            {
               "row":2,
               "column":5
            },
            {
               "row":3,
               "column":5
            }
         ]
      },
      {
         "coordinates":[
            {
               "row":1,
               "column":6
            },
            {
               "row":2,
               "column":6
            },
            {
               "row":3,
               "column":6
            }
         ]
      },
      {
         "coordinates":[
            {
               "row":1,
               "column":7
            },
            {
               "row":2,
               "column":7
            },
            {
               "row":3,
               "column":7
            }
         ]
      },
      {
         "coordinates":[
            {
               "row":1,
               "column":8
            },
            {
               "row":2,
               "column":8
            }
         ]
      },
      {
         "coordinates":[
            {
               "row":1,
               "column":9
            },
            {
               "row":2,
               "column":9
            }
         ]
      },
      {
         "coordinates":[
            {
               "row":8,
               "column":5
            },
            {
               "row":9,
               "column":5
            }
         ]
      },
      {
         "coordinates":[
            {
               "row":8,
               "column":6
            },
            {
               "row":9,
               "column":6
            }
         ]
      }
   ]
}
```

Same ships can be placed by player with Id 2


```POST https://localhost:5001/battleship/shot```

Starting with Player 1, performs shot at the given coordinate.

Sample request:

```
{
    "gameId": 8843,
    "playerId": 1,
    "coordinate": {
         "row": 4,
         "column": 3
    }
}
```

Both POST methods return the game status as response, example:

```
{
    "GameId": 8843,
    "GameStatus": "SHOOTING",
    "NextPlayer": 2,
    "WinnerPlayer": 0,
    "Message": "Shot succeeded"
}
```

After one of the players sunks all the ships of their opponent, the game goes into "FINISHED" state and the winner is announced:

```
{
    "GameId": 8843,
    "GameStatus": "FINISHED",
    "NextPlayer": 2,
    "WinnerPlayer": 1,
    "Message": "Game finished, player 1 has won the game"
}
```


## License
[MIT](https://choosealicense.com/licenses/mit/)