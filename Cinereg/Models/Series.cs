using System.ComponentModel.DataAnnotations;

namespace Cinereg.Models
{
    public class Series
    {
        [Key]
        public string? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int StartYear { get; set; } = DateTime.Now.Year;
        public string? UserId { get; set; }
        [Required]
        public int EndYear { get; set; } = DateTime.Now.Year;
        public string ViewingForm { get; set; }
        [Required]
        public string Review { get; set; }

        public List<Season> Seasons { get; set; } = new() { new() };
        public List<Genre> SeriesGenres { get; set; } = new() { new() { Name = string.Empty } };
    }
}
