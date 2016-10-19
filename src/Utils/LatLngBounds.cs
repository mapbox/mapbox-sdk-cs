//-----------------------------------------------------------------------
// <copyright file="LatLngBounds.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox
{
    /// <summary> Represents a bounding box derived from a southwest corner and a northeast corner. </summary>
    public struct LatLngBounds
    {
        /// <summary> Southwest corner of bounding box. </summary>
        public LatLng SouthWest;

        /// <summary> Northeast corner of bounding box. </summary>
        public LatLng NorthEast;

        /// <summary> Initializes a new instance of the <see cref="LatLngBounds" /> struct. </summary>
        /// <param name="sw"> Geographic coordinate representing southwest corner of bounding box. </param>
        /// <param name="ne"> Geographic coordinate representing northeast corner of bounding box. </param>
        public LatLngBounds(LatLng sw, LatLng ne)
        {
            this.SouthWest = sw;
            this.NorthEast = ne;
        }

        /// <summary> Converts the Bbox to a URL snippet. </summary>
        /// <returns> Returns a string for use in a Mapbox query URL. </returns>
        public override string ToString()
        {
            return this.SouthWest.ToString() + "," + this.NorthEast.ToString();
        }
    }
}