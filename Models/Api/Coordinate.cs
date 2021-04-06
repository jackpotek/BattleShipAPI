namespace Battleships.Models.Api
{
    public class Coordinate
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Coordinate(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public Coordinate()
        {

        }
    }
}
