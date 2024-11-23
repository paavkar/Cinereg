using System.ComponentModel.DataAnnotations;

namespace Cinereg.Models
{
    public class Genre
    {
        [Key]
        public string Id { get; set; } = Guid.CreateVersion7().ToString();
        public required string Name { get; set; }
    }
}
