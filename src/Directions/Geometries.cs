//-----------------------------------------------------------------------
// <copyright file="Geometries.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Directions
{
    /// <summary> Format of the returned geometries in the route overview and steps. </summary>
    public sealed class Geometries
    {
        /// <summary> Polyline geometry type. </summary>
        public static readonly Geometries Polyline = new Geometries("polyline");

        /// <summary> Geojson geometry type. </summary>
        public static readonly Geometries Geojson = new Geometries("geojson");

        private readonly string geometry;

        private Geometries(string geometry)
        {
            this.geometry = geometry;
        }

        /// <summary> Converts the geometry to a string. </summary>
        /// <returns> A string to use as an optional value in the direction query URL. </returns>
        public override string ToString()
        {
            return this.geometry;
        }
    }
}
