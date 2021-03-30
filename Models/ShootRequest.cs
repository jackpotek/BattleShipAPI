using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleships.Models
{
    public class ShootRequest
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }

        public Coordinate Coordinate { get; set; }

        public string validateShot()
        {
            if(Coordinate.Row < 1 || Coordinate.Row > 10 || Coordinate.Column < 1 || Coordinate.Column > 10)
                return "incorrect coordinates";
            else
                return "";
        }

    }
}
