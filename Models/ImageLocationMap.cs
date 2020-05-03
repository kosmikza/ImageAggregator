using System.ComponentModel.DataAnnotations;

namespace ImmageAggregatorAPI.Models
{
    public class ImageLocationMap
    {
        [Key]
        [Required]
        public long ImageLocationMapId { get; set; }
        [Required]
        public long LocationId { get; set; }
        [Required]
        public long ImageId { get; set; }
    }
}
