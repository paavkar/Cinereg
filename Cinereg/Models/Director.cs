using System.ComponentModel.DataAnnotations;

namespace Cinereg.Models
{
    public class Director
    {
        [Key]
        public string Id { get; set; } = Guid.CreateVersion7().ToString();
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
