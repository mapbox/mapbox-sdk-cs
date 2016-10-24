//-----------------------------------------------------------------------
// <copyright file="VectorTile.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    /// <summary>
    ///    A decoded vector tile, as specified by the
    ///    <see href="https://www.mapbox.com/vector-tiles/specification/">
    ///    Mapbox Vector Tile specification </see>. The tile might be
    ///    incomplete if the network request and parsing are still pending.
    /// </summary>
    public sealed class VectorTile : Tile
    {
        private byte[] data;

        /// <summary> Gets the vector tile raw data. </summary>
        /// <value> The raw data. </value>
        public byte[] Data
        {
            get
            {
                return this.data;
            }
        }

        internal override TileResource MakeTileResource(string source)
        {
            return TileResource.MakeVector(Id, source);
        }

        internal override bool ParseTileData(byte[] data)
        {
            // TODO: Parse.
            this.data = data;

            return true;
        }
    }
}
