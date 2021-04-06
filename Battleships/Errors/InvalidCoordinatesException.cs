namespace Battleships.Errors
{
    public class InvalidCoordinatesException : InvalidRequestException
    {
        public InvalidCoordinatesException(int? width, int? height)
            : base($"Invalid coordinates supplied, possible values: columns 1-{width}, rows 1-{height}")
        {
        }
    }
}
