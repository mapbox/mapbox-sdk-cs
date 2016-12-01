//-----------------------------------------------------------------------
// <copyright file="JsonConverters.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox
{
    using Mapbox.Json;

    /// <summary>
    /// Custom json converters.
    /// </summary>
    public static class JsonConverters
    {
        /// <summary>
        /// Array of converters.
        /// </summary>
        private static JsonConverter[] converters =
        {
            new LonLatToGeoCoordinateConverter(),
            new BboxToGeoCoordinateBoundsConverter(),
            new PolylineToGeoCoordinateListConverter()
        };

        /// <summary>
        /// Gets the converters.
        /// </summary>
        /// <value>The converters.</value>
        public static JsonConverter[] Converters
        {
            get
            {
                return converters;
            }
        }
    }
}
