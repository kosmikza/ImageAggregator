﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using FourSquareModels;
//
//    var venueSearch = VenueSearch.FromJson(jsonString);

namespace FourSquareModels.VenueSearch
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class VenueSearch
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public partial class Meta
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("venues")]
        public List<Venue> Venues { get; set; }

        [JsonProperty("geocode")]
        public Geocode Geocode { get; set; }
    }

    public partial class Geocode
    {
        [JsonProperty("what")]
        public string What { get; set; }

        [JsonProperty("where")]
        public string Where { get; set; }

        [JsonProperty("feature")]
        public Feature Feature { get; set; }

        [JsonProperty("parents")]
        public List<object> Parents { get; set; }
    }

    public partial class Feature
    {
        [JsonProperty("cc")]
        public string Cc { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("matchedName")]
        public string MatchedName { get; set; }

        [JsonProperty("highlightedName")]
        public string HighlightedName { get; set; }

        [JsonProperty("woeType")]
        public long WoeType { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("longId")]
        public string LongId { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }
    }

    public partial class Geometry
    {
        [JsonProperty("center")]
        public Center Center { get; set; }

        [JsonProperty("bounds")]
        public Bounds Bounds { get; set; }
    }

    public partial class Bounds
    {
        [JsonProperty("ne")]
        public Center Ne { get; set; }

        [JsonProperty("sw")]
        public Center Sw { get; set; }
    }

    public partial class Center
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    public partial class Venue
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("categories")]
        public List<Category> Categories { get; set; }

        [JsonProperty("referralId")]
        public string ReferralId { get; set; }

        [JsonProperty("hasPerk")]
        public bool HasPerk { get; set; }
    }

    public partial class Category
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pluralName")]
        public string PluralName { get; set; }

        [JsonProperty("shortName")]
        public string ShortName { get; set; }

        [JsonProperty("icon")]
        public Icon Icon { get; set; }

        [JsonProperty("primary")]
        public bool Primary { get; set; }
    }

    public partial class Icon
    {
        [JsonProperty("prefix")]
        public Uri Prefix { get; set; }

        [JsonProperty("suffix")]
        public string Suffix { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }

        [JsonProperty("postalCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? PostalCode { get; set; }

        [JsonProperty("cc")]
        public string Cc { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("formattedAddress")]
        public List<string> FormattedAddress { get; set; }
    }

    public partial class VenueSearch
    {
        public static VenueSearch FromJson(string json) => JsonConvert.DeserializeObject<VenueSearch>(json, FourSquareModels.VenueSearch.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this VenueSearch self) => JsonConvert.SerializeObject(self, FourSquareModels.VenueSearch.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
