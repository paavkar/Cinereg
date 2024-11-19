using System.ComponentModel.DataAnnotations;

namespace Cinereg.Models
{
    public class Genre
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Name { get; set; }
    }
}
