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

        public override bool Equals(object obj) => (this.Row == (obj as Coordinate).Row && this.Column == (obj as Coordinate).Column);
        public override int GetHashCode() => (Row, Column).GetHashCode();
    }
}
