using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinereg.Entities
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ReleaseYear { get; set; } = DateTime.Now.Year;
        [Required]
        public string Genre { get; set; }
        [Required]
        public string Director { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int WatchedYear { get; set; } = DateTime.Now.Year;
        [Required]
        public string ViewingForm { get; set; }
        [Required]
        public string Review { get; set; }
    }
}
