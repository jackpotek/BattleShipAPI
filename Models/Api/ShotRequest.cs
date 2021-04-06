namespace Battleships.Models.Api
{
    public class ShotRequest
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }

        public Coordinate Coordinate { get; set; }

    }
}
