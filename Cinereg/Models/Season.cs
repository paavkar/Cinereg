using System.ComponentModel.DataAnnotations;

namespace Cinereg.Models
{
    public class Season
    {
        [Key]
        public string Id { get; set; }
        public string ShowId { get; set; }
        [Required]
        public int SeasonNumber { get; set; }
        [Required]
        public int NumberOfEpisodes { get; set; }
        [Required]
        public double Rating { get; set; }
        [Required]
        public string Review { get; set; }

        public List<Director> Directors { get; set; }
    }
}
