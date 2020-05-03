using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImmageAggregatorAPI.Models
{
    public class Image
    {
        [Required]
        [Key]
        public long ImageId { get; set; }
        [Required]
        public string ImageName { get; set; }
        public string ImageWebURL { get; set; }
        public long? ImageBlobId { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string ImageURLPrefix { get; set; }
        public string ImageURLSuffix { get; set; }
        public string FourSquareVenueId { get; set; }
        public string GooglePlaceId { get; set; }
        public string FourSquareImageId { get; set; }
        public string GoogleImageId { get; set; }
        public int ImageApiSource { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

    }

    public class ImageComplex
    {
        public Image ImageMeta { get; set; }
        public List<Location> LinkedLocations { get; set; }
        public ImageBlob Blob { get; set; }
    }

    public class ImageBlob
    {
        [Key]
        [Required]
        public long ImageBlobId { get; set; }
        public byte[] Blob { get; set; }
    }

    public enum ImageApiSource
    {
        FourSquare = 1,
        Google = 2
    }



}
