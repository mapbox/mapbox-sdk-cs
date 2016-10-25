//-----------------------------------------------------------------------
// <copyright file="VectorTile.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    using Mapbox.Utils;
    using Mapbox.VectorTile;

    /// <summary>
    ///    A decoded vector tile, as specified by the
    ///    <see href="https://www.mapbox.com/vector-tiles/specification/">
    ///    Mapbox Vector Tile specification </see>. The tile might be
    ///    incomplete if the network request and parsing are still pending.
    /// </summary>
    public sealed class VectorTile : Tile
    {
        // FIXME: Namespace here is very confusing and conflicts (sematically)
        // with his class. Something has to be renamed here.
        private Mapbox.VectorTile.VectorTile data;

        /// <summary> Gets the vector decoded using Mapbox.VectorTile library. </summary>
        /// <value> The GeoJson data. </value>
        public Mapbox.VectorTile.VectorTile Data
        {
            get
            {
                return this.data;
            }
        }

        /// <summary> Gets the vector in a GeoJson format. </summary>
        /// <value> The GeoJson data. </value>
        public string GeoJson
        {
            get
            {
                return this.data.ToGeoJson();
            }
        }

        internal override TileResource MakeTileResource(string source)
        {
            return TileResource.MakeVector(Id, source);
        }

        internal override bool ParseTileData(byte[] data)
        {
            try
            {
                // TODO: Move this to a threaded worker.
                var decompressed = Compression.Decompress(data);
                this.data = VectorTileReader.Decode((ulong)Id.Z, (ulong)Id.X, (ulong)Id.Y, decompressed);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
