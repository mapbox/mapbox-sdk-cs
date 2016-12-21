//-----------------------------------------------------------------------
// <copyright file="RawPngRasterTile.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    /// <summary>
    ///    A raster tile containing an encoded RGBA PNG.
    /// </summary>
    public sealed class RawPngRasterTile : RasterTile
    {
        internal override TileResource MakeTileResource(string mapId)
        {
            return TileResource.MakeRawPngRaster(Id, mapId);
        }
    }
}
