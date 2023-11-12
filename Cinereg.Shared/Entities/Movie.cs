namespace Cinereg.Shared.Entities
{
    public class Movie
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public required int ReleaseYear { get; set; }
        public required string Genre { get; set; }
        public required string Director { get; set; }
        public required string UserId { get; set; }
    }
}
