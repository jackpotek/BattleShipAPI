using Battleships.Errors;

namespace Battleships.Models.Api
{
    public class ShotRequest
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }

        public Coordinate Coordinate { get; set; }

        public void validateShot(int width, int height)
        {
            if (Coordinate.Row < 1 || Coordinate.Row > height || Coordinate.Column < 1 || Coordinate.Column > width)
                throw new InvalidCoordinatesException(width, height);
        }

    }
}
