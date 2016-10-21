//-----------------------------------------------------------------------
// <copyright file="Tile.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    /// <summary>
    ///    A Map tile, a square with vector or raster data representing a geographic
    ///    bounding box. More info <see href="https://en.wikipedia.org/wiki/Tiled_web_map">
    ///    here </see>.
    /// </summary>
    public abstract class Tile
    {
        private CanonicalTileId id;
        private IAsyncRequest request;

        /// <summary> Gets the canonical tile identifier. </summary>
        /// <value> The canonical tile identifier. </value>
        public CanonicalTileId Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary> Cancel any further processing on this tile. </summary>
        public void Cancel()
        {
            if (this.request != null)
            {
                this.request.Cancel();
                this.request = null;
            }
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String"/> that represents the current
        ///     <see cref="T:Mapbox.Map.Tile"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String"/> that represents the current
        ///     <see cref="T:Mapbox.Map.Tile"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Id.ToString();
        }

        // TODO: Currently the tile decoding is done on the main thread. We must implement
        // a Worker class to abstract this, so on platforms that support threads (like Unity
        // on the desktop, Android, etc) we can use worker threads and when building for
        // the browser, we keep it single-threaded.
        internal void Initialize(CanonicalTileId id, IFileSource fs)
        {
            this.id = id;
            this.request = fs.Request(this.MakeTileResource().GetUrl(), this.HandleTileResponse);
        }

        internal abstract TileResource MakeTileResource();

        // Decode the tile.
        internal abstract bool ParseTileData(byte[] data);

        private void HandleTileResponse(Response response)
        {
            this.ParseTileData(response.Data);
        }
    }
}
