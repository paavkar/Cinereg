namespace Cinereg.Models
{
    public class MovieWithGenres : Movie
    {
        public List<Genre> MovieGenres { get; set; } = new() { new() { Name = string.Empty } };
    }
}
