//-----------------------------------------------------------------------
// <copyright file="RasterTile.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    /// <summary>
    ///    A raster tile from the Mapbox Style API, a encoded image representing a geographic
    ///    bounding box. Usually JPEG or PNG encoded.
    /// </summary>
    public class RasterTile : Tile
    {
        private byte[] data;

        /// <summary> Gets the raster tile raw data. </summary>
        /// <value> The raw data, usually an encoded JPEG or PNG. </value>
        public byte[] Data
        {
            get
            {
                return this.data;
            }
        }

        internal override TileResource MakeTileResource(string styleUrl)
        {
            return TileResource.MakeRaster(Id, styleUrl);
        }

        internal override bool ParseTileData(byte[] data)
        {
            // We do not parse raster tiles as they are
            // decoded by Unity on `Texture2D.LoadImage`.
            this.data = data;

            return true;
        }
    }
}
