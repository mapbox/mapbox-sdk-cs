//-----------------------------------------------------------------------
// <copyright file="Route.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Directions
{
    using System.Collections.Generic;
    using Mapbox.Json;

    /// <summary>
    /// A Route from a Directions API call.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Gets or sets the legs.
        /// </summary>
        /// <value>The legs.</value>
        [JsonProperty("legs")]
        public List<Leg> Legs { get; set; }

        /// <summary>
        /// Gets or sets the geometry. Polyline is an array of LatLng's.
        /// </summary>
        /// <value>The geometry.</value>
        [JsonProperty("geometry")]
        [JsonConverter(typeof(PolylineToGeoCoordinateListConverter))]
        public List<GeoCoordinate> Geometry { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        [JsonProperty("duration")]
        public double Duration { get; set; }

        /// <summary>
        /// Gets or sets the distance.
        /// </summary>
        /// <value>The distance.</value>
        [JsonProperty("distance")]
        public double Distance { get; set; }
    }
}