using System.Collections.Generic;

namespace ImmageAggregatorAPI.Models.FourSquare
{

    public class Meta
    {
        public int code { get; set; }
        public string requestId { get; set; }
    }

    public class Photo
    {
        public string prefix { get; set; }
        public string suffix { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Photo photo { get; set; }
    }

    public class PhotoItem
    {
        public string id { get; set; }
        public int createdAt { get; set; }
        public string prefix { get; set; }
        public string suffix { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public User user { get; set; }
        public string visibility { get; set; }
    }

    public class Photos
    {
        public int count { get; set; }
        public IList<PhotoItem> items { get; set; }
    }

    public class PhotoResponse
    {
        public Photos photos { get; set; }
    }

    public class GetVenuePhotoResponse
    {
        public Meta meta { get; set; }
        public PhotoResponse response { get; set; }
    }

    public class LabeledLatLng
    {
        public string label { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Location
    {
        public string address { get; set; }
        public string crossStreet { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public IList<LabeledLatLng> labeledLatLngs { get; set; }
        public string postalCode { get; set; }
        public string cc { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public IList<string> formattedAddress { get; set; }
    }

    public class Icon
    {
        public string prefix { get; set; }
        public string suffix { get; set; }
    }

    public class Category
    {
        public string id { get; set; }
        public string name { get; set; }
        public string pluralName { get; set; }
        public string shortName { get; set; }
        public Icon icon { get; set; }
        public bool primary { get; set; }
    }

    public class Venue
    {
        public string id { get; set; }
        public string name { get; set; }
        public Location location { get; set; }
        public IList<Category> categories { get; set; }
        public string referralId { get; set; }
        public bool hasPerk { get; set; }
    }

    public class Center
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Ne
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Sw
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Bounds
    {
        public Ne ne { get; set; }
        public Sw sw { get; set; }
    }

    public class Geometry
    {
        public Center center { get; set; }
        public Bounds bounds { get; set; }
    }

    public class Feature
    {
        public string cc { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public string matchedName { get; set; }
        public string highlightedName { get; set; }
        public int woeType { get; set; }
        public string slug { get; set; }
        public string id { get; set; }
        public string longId { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Geocode
    {
        public string what { get; set; }
        public string where { get; set; }
        public Feature feature { get; set; }
        public IList<object> parents { get; set; }
    }

    public class VenueResponse
    {
        public IList<Venue> venues { get; set; }
        public Geocode geocode { get; set; }
    }

    public class GetVenueResponse
    {
        public Meta meta { get; set; }
        public VenueResponse response { get; set; }
    }

}
