using System.ComponentModel.DataAnnotations;

namespace ImmageAggregatorAPI.Models
{
    public class Location
    {
        [Key]
        public long LocationId { get; set; }
        [Required]
        public string LocationName { get; set; }
        public string LocationCountryCode { get; set; }
        public double? LongitudeDecimal { get; set; }
        public double? LatitudeDecimal { get; set; }
        public bool ImageLoadingComplete { get; set; }
        public string LastRunStatus { get; set; }
    }

    public class LocationDTO
    {
        public string LocationName { get; set; }
        public string LocationCountryCode { get; set; }
        public double? LongitudeDecimal { get; set; }
        public double? LatitudeDecimal { get; set; }

    }


}
