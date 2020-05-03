using System.Collections.Generic;

namespace ImmageAggregatorAPI.Models.GoogleMaps
{
    public class GoogleMaps
    {

        public class Location
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Northeast
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Southwest
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Viewport
        {
            public Northeast northeast { get; set; }
            public Southwest southwest { get; set; }
        }

        public class Geometry
        {
            public Location location { get; set; }
            public Viewport viewport { get; set; }
        }

        public class Photo
        {
            public int height { get; set; }
            public IList<string> html_attributions { get; set; }
            public string photo_reference { get; set; }
            public int width { get; set; }
        }

        public class GooglePlaceNameSearchCandidate
        {
            public string formatted_address { get; set; }
            public Geometry geometry { get; set; }
            public string name { get; set; }
            public IList<Photo> photos { get; set; }
        }

        public class GooglePlaceNameSearch
        {
            public IList<GooglePlaceNameSearchCandidate> candidates { get; set; }
            public string status { get; set; }
        }

        public class GooglePlaceSearchResult
        {
            public string formatted_address { get; set; }
            public Geometry geometry { get; set; }
            public string icon { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public IList<Photo> photos { get; set; }
            public string place_id { get; set; }
            public string reference { get; set; }
            public IList<string> types { get; set; }
        }

        public class GooglePlaceSearch
        {
            public IList<object> html_attributions { get; set; }
            public IList<GooglePlaceSearchResult> results { get; set; }
            public string status { get; set; }
        }
    }
}
