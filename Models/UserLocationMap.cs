using System;
using System.ComponentModel.DataAnnotations;

namespace ImmageAggregatorAPI.Models
{
    public class UserLocationMap
    {
        [Key]
        [Required]
        public long UserLocationMapId { get; set; }
        public long UserId { get; set; }
        public long LocationId { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? RemovedOn { get; set; }
    }

    public class UserLocationMapDTO
    {
        public long LocationId { get; set; }
    }
}
