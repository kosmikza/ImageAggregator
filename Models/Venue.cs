using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IMMediaAPI.Models
{
    public class Venue
    {
        [Key]
        [Required]
        public long VenueId { get; set; }
        public long LinkedLocationId { get; set; }
        public string FourSquareGeoCodeId { get; set; }
        public string FourSquareVenueId { get; set; }
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }

    }
}
