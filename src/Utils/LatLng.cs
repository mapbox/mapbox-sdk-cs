//-----------------------------------------------------------------------
// <copyright file="LatLng.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox
{
    /// <summary> Represents a geographic coordinate. </summary>
    public struct LatLng
    {
        /// <summary> Latitude, in decimal degrees. </summary>
        public double Latitude;

        /// <summary> Longitude, in decimal degrees. </summary>
        public double Longitude;

        /// <summary> Initializes a new instance of the <see cref="LatLng" /> struct. </summary>
        /// <param name="latitude"> Latitude, in decimal degrees. </param>
        /// <param name="longitude"> Longitude, in decimal degrees. </param>
        public LatLng(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }
}