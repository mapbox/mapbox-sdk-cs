//-----------------------------------------------------------------------
// <copyright file="Map.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The Mapbox Map abstraction will take care of fetching and decoding
    ///     data for a geographic bounding box at a certain zoom level.
    /// </summary>
    /// <typeparam name="T">
    ///     The tile type, currently <see cref="T:Mapbox.Map.Vector"/> or
    ///     <see cref="T:Mapbox.Map.Raster"/>.
    /// </typeparam>
    public sealed class Map<T> where T : Tile, new()
    {
        private readonly IFileSource fs;
        private GeoCoordinateBounds latLngBounds;
        private double zoom;
        private HashSet<T> tiles = new HashSet<T>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Mapbox.Map.Map`1"/> class.
        /// </summary>
        /// <param name="fs"> The data source abstraction. </param>
        public Map(IFileSource fs)
        {
            this.fs = fs;
            this.GeoCoordinateBounds = new GeoCoordinateBounds();
            this.zoom = 0;
        }

        /// <summary>
        ///     Gets the tiles, vector or raster. Tiles might be
        ///     in a incomplete state.
        /// </summary>
        /// <value> The tiles. </value>
        public HashSet<T> Tiles
        {
            get
            {
                return this.tiles;
            }
        }

        /// <summary>Gets or sets a geographic bounding box.</summary>
        /// <value>New geographic bounding box.</value>
        public GeoCoordinateBounds GeoCoordinateBounds
        {
            get
            {
                return this.latLngBounds;
            }

            set
            {
                this.latLngBounds = value;
                this.Update();
            }
        }

        /// <summary>Gets or sets the central coordinate of the map.</summary>
        /// <value>The central coordinate.</value>
        public GeoCoordinate Center
        {
            get
            {
                return this.latLngBounds.Center;
            }

            set
            {
                this.latLngBounds.Center = value;
                this.Update();
            }
        }

        /// <summary>Gets or sets the map zoom level.</summary>
        /// <value>The new zoom level.</value>
        public double Zoom
        {
            get
            {
                return this.zoom;
            }

            set
            {
                this.zoom = value;
                this.Update();
            }
        }

        private void Update()
        {
            var cover = TileCover.Get(this.latLngBounds, (int)Math.Ceiling(this.zoom));

            // Do not request tiles that we are already requesting
            // but at the same time exclude the ones we don't need
            // anymore, cancelling the network request.
            this.tiles.RemoveWhere((T tile) =>
                {
                    if (cover.Remove(tile.Id))
                    {
                        return false;
                    }
                    else
                    {
                        tile.Cancel();
                        return true;
                    }
                });

            foreach (CanonicalTileId id in cover)
            {
                var tile = new T();
                tile.Initialize(id, this.fs);

                this.tiles.Add(tile);
            }
        }
    }
}
