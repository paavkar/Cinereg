using System.ComponentModel.DataAnnotations;

namespace Cinereg.Models
{
    public class Genre
    {
        [Key]
        public string Id { get; set; }
        public required string Name { get; set; }
    }
}
