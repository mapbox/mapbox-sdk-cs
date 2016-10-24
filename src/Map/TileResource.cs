//-----------------------------------------------------------------------
// <copyright file="TileResource.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    using Platform;

    internal sealed class TileResource : IResource
    {
        private const string Api = "https://api.mapbox.com/v4/";
        private readonly string query;

        internal TileResource(string query)
        {
            this.query = query;
        }

        public static TileResource MakeRaster(CanonicalTileId id, string source)
        {
            return new TileResource(string.Format("{0}/{1}.png", source ?? "mapbox.satellite", id));
        }

        public static TileResource MakeRawPngRaster(CanonicalTileId id, string source)
        {
            return new TileResource(string.Format("{0}/{1}.pngraw", source ?? "mapbox.terrain-rgb", id));
        }

        public static TileResource MakeVector(CanonicalTileId id, string source)
        {
            return new TileResource(string.Format("{0}/{1}.vector.pbf", source ?? "mapbox.mapbox-streets-v7", id));
        }

        public string GetUrl()
        {
            return Api + this.query;
        }
    }
}
