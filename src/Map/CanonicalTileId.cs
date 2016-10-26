//-----------------------------------------------------------------------
// <copyright file="CanonicalTileId.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    using System;

    /// <summary>
    ///     Canonical tile identifier in a slippy map.
    /// </summary>
    public struct CanonicalTileId
    {
        /// <summary> The zoom level. </summary>
        public readonly int Z;

        /// <summary> The X coordinate in the tile grid. </summary>
        public readonly int X;

        /// <summary> The Y coordinate in the tile grid. </summary>
        public readonly int Y;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CanonicalTileId"/> struct,
        ///     representing a tile coordinate in a slippy map.
        /// </summary>
        /// <param name="z"> The z coordinate or the zoom level. </param>
        /// <param name="x"> The x coordinate. </param>
        /// <param name="y"> The y coordinate. </param>
        public CanonicalTileId(int z, int x, int y)
        {
            this.Z = z;
            this.X = x;
            this.Y = y;
        }

        internal CanonicalTileId(UnwrappedTileId unwrapped)
        {
            var z = unwrapped.Z;
            var x = unwrapped.X;
            var y = unwrapped.Y;

            var wrap = (x < 0 ? x - (1 << z) + 1 : x) / (1 << z);

            this.Z = z;
            this.X = x - wrap * (1 << z);
            this.Y = y < 0 ? 0 : Math.Min(y, (1 << z) - 1);
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String"/> that represents the current
        ///     <see cref="T:Mapbox.Map.CanonicalTileId"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String"/> that represents the current
        ///     <see cref="T:Mapbox.Map.CanonicalTileId"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Z + "/" + this.X + "/" + this.Y;
        }
    }
}
