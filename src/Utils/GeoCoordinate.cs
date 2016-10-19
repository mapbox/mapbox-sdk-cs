//-----------------------------------------------------------------------
// <copyright file="GeoCoordinate.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox
{
    /// <summary> Represents a geographic coordinate. </summary>
    public struct GeoCoordinate
    {
        /// <summary> Latitude, in decimal degrees. </summary>
        public double Latitude;

        /// <summary> Longitude, in decimal degrees. </summary>
        public double Longitude;

        /// <summary> Initializes a new instance of the <see cref="GeoCoordinate" /> struct. </summary>
        /// <param name="latitude"> Latitude, in decimal degrees. </param>
        /// <param name="longitude"> Longitude, in decimal degrees. </param>
        public GeoCoordinate(double latitude, double longitude)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
        }

        /// <summary> Converts LatLng to a URL snippet. </summary>
        /// <returns> Returns a string for use in a Mapbox query URL. </returns>
        public override string ToString()
        {
            return string.Format("{0:F5},{1:F5}", this.Longitude, this.Latitude);
        }
    }
}
