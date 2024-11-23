using System.ComponentModel.DataAnnotations;

namespace Cinereg.Models
{
    public class Season
    {
        [Key]
        public string Id { get; set; } = Guid.CreateVersion7().ToString();
        public string ShowId { get; set; }
        [Required]
        public int SeasonNumber { get; set; }
        [Required]
        public int NumberOfEpisodes { get; set; }
        [Required]
        public double Rating { get; set; }
        public string Review { get; set; }
        [Required]
        public int ReleaseYear { get; set; } = DateTime.Now.Year;
        [Required]
        public int WatchedYear { get; set; } = DateTime.Now.Year;
        [Required]
        public string ViewingForm { get; set; }

        public List<Director> Directors { get; set; }
    }
}
