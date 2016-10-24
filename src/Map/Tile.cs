//-----------------------------------------------------------------------
// <copyright file="Tile.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    using System;

    /// <summary>
    ///    A Map tile, a square with vector or raster data representing a geographic
    ///    bounding box. More info <see href="https://en.wikipedia.org/wiki/Tiled_web_map">
    ///    here </see>.
    /// </summary>
    public abstract class Tile
    {
        private CanonicalTileId id;
        private string error;
        private bool loaded = false;

        private IAsyncRequest request;
        private Action<Tile> callback;

        /// <summary> Gets the canonical tile identifier. </summary>
        /// <value> The canonical tile identifier. </value>
        public CanonicalTileId Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary> Gets the error message if any. </summary>
        /// <value> The error string. </value>
        public string Error
        {
            get
            {
                return this.error;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the tile is loaded. A loaded
        ///     tile doesn't necessarily contain data, as it could have error'ed.
        /// </summary>
        /// <value> True if loaded, false otherwise. </value>
        public bool Loaded
        {
            get
            {
                return this.loaded;
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

        internal void Cancel()
        {
            if (this.request != null)
            {
                this.request.Cancel();
                this.request = null;
            }
        }

        // TODO: Currently the tile decoding is done on the main thread. We must implement
        // a Worker class to abstract this, so on platforms that support threads (like Unity
        // on the desktop, Android, etc) we can use worker threads and when building for
        // the browser, we keep it single-threaded.
        internal void Initialize(Parameters param, Action<Tile> callback)
        {
            this.id = param.Id;
            this.request = param.Fs.Request(this.MakeTileResource(param.Source).GetUrl(), this.HandleTileResponse);
            this.callback = callback;
        }

        // Get the tile resource (raster/vector/etc).
        internal abstract TileResource MakeTileResource(string source);

        // Decode the tile.
        internal abstract bool ParseTileData(byte[] data);

        private void HandleTileResponse(Response response)
        {
            if (response.Error != null)
            {
                this.error = response.Error;
            } 
            else if (this.ParseTileData(response.Data) == false)
            {
                this.error = "Parse error.";
            }

            this.loaded = true;
            this.callback(this);
        }

        internal struct Parameters
        {
            public CanonicalTileId Id;
            public string Source;
            public IFileSource Fs;
        }
    }
}
