using System.ComponentModel.DataAnnotations;

namespace Cinereg.Models
{
    public class Show
    {
        [Key]
        public string? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ReleaseYear { get; set; } = DateTime.Now.Year;
        public string? UserId { get; set; }
        [Required]
        public int WatchedYear { get; set; } = DateTime.Now.Year;
        [Required]
        public string ViewingForm { get; set; }
        [Required]
        public string Review { get; set; }

        public List<Season> Seasons { get; set; }
        public List<Genre> ShowGenres { get; set; } = new() { new() { Name = string.Empty } };
    }
}
