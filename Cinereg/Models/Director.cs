using System.ComponentModel.DataAnnotations;

namespace Cinereg.Models
{
    public class Director
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
