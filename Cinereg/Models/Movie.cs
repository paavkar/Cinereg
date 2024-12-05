using System.ComponentModel.DataAnnotations;

namespace Cinereg.Models
{
    public class Movie
    {
        [Key]
        public string? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ReleaseYear { get; set; } = DateTime.Now.Year;
        [Required]
        public string Director { get; set; }
        public string? UserId { get; set; }
        [Required]
        public int WatchedYear { get; set; } = DateTime.Now.Year;
        [Required]
        public string ViewingForm { get; set; }
        [Required]
        public string Review { get; set; }

        public List<Genre> MovieGenres { get; set; } = new() { new() { Name = string.Empty } };
        public List<Director> Directors { get; set; } = new() { new() };
    }
}
