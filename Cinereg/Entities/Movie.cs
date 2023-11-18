using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinereg.Entities
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public required string Name { get; set; }
        public required int ReleaseYear { get; set; }
        public required string Genre { get; set; }
        public required string Director { get; set; }
        public required string UserId { get; set; }
        public required int WatchedYear { get; set; }
        public required string ViewingForm { get; set; }
        public required string Review { get; set; }
    }
}
