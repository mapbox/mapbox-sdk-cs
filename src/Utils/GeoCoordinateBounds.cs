//-----------------------------------------------------------------------
// <copyright file="GeoCoordinateBounds.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox
{
    /// <summary> Represents a bounding box derived from a southwest corner and a northeast corner. </summary>
    public struct GeoCoordinateBounds
    {
        /// <summary> Southwest corner of bounding box. </summary>
        public GeoCoordinate SouthWest;

        /// <summary> Northeast corner of bounding box. </summary>
        public GeoCoordinate NorthEast;

        /// <summary> Initializes a new instance of the <see cref="GeoCoordinateBounds" /> struct. </summary>
        /// <param name="sw"> Geographic coordinate representing southwest corner of bounding box. </param>
        /// <param name="ne"> Geographic coordinate representing northeast corner of bounding box. </param>
        public GeoCoordinateBounds(GeoCoordinate sw, GeoCoordinate ne)
        {
            this.SouthWest = sw;
            this.NorthEast = ne;
        }

        /// <summary> Converts the Bbox to a URL snippet. </summary>
        /// <returns> Returns a string for use in a Mapbox query URL. </returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", this.SouthWest.ToString(), this.NorthEast.ToString());
        }
    }
}