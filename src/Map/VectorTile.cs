//-----------------------------------------------------------------------
// <copyright file="VectorTile.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    using System.Collections.ObjectModel;
    using Mapbox.Utils;
    using Mapbox.VectorTile;
    using Mapbox.VectorTile.ExtensionMethods;

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

        /// <summary>
        /// <para>Gets the vector in a GeoJson format.</para>
        /// <para>
        /// This method should be avoided as it fully decodes the whole tile and might pose performance and memory bottle necks.
        /// </para>
        /// </summary>
        /// <value> The GeoJson data. </value>
        public string GeoJson
        {
            get
            {
                return this.data.ToGeoJson((ulong)Id.Z, (ulong)Id.X, (ulong)Id.Y, 0);
            }
        }

        /// <summary>
        /// Gets all availble layer names.
        /// </summary>
        /// <returns>Collection of availble layers.</returns>
        public ReadOnlyCollection<string> LayerNames()
        {
            return this.data.LayerNames();
        }

        /// <summary>
        /// Decodes the requested layer.
        /// </summary>
        /// <param name="layerName">Name of the layer to decode.</param>
        /// <returns>Decoded VectorTileLayer or 'null' if an invalid layer name was specified.</returns>
        public VectorTileLayer GetLayer(string layerName)
        {
            return this.data.GetLayer(layerName);
        }

        internal override TileResource MakeTileResource(string mapId)
        {
            return TileResource.MakeVector(Id, mapId);
        }

        internal override bool ParseTileData(byte[] data)
        {
            try
            {
                // TODO: Move this to a threaded worker.
                var decompressed = Compression.Decompress(data);
                this.data = new Mapbox.VectorTile.VectorTile(decompressed);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
