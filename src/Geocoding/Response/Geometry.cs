//-----------------------------------------------------------------------
// <copyright file="Geometry.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Geocoding
{
    using System;
    using Mapbox.Json;

    /// <summary> Point geometry representing location of geocode result. </summary>
    [Serializable]
    public class Geometry
    {
        /// <summary>
        ///     Gets or sets type. Geocode results will always be type: point.
        /// </summary>
        /// <value>The GeoJSON geometry type.</value>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        ///     Gets or sets coordinates. Because they are points, Geocode results will always be  a single Geocoordinate.
        /// </summary>
        /// <value>The coordinates.</value>
        [JsonConverter(typeof(LonLatToGeoCoordinateConverter))]
        [JsonProperty("coordinates")]
        public GeoCoordinate Coordinates { get; set; }
    }
}
