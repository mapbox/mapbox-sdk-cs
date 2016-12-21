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
    public sealed class Map<T> : Mapbox.IObservable<T> where T : Tile, new()
    {
        /// <summary>
        ///     Arbitrary limit of tiles this class will handle simultaneously.
        /// </summary>
        public const int TileMax = 256;

        private readonly IFileSource fs;
        private GeoCoordinateBounds latLngBounds;
        private int zoom;
        private string mapId;

        private HashSet<T> tiles = new HashSet<T>();
        private List<Mapbox.IObserver<T>> observers = new List<Mapbox.IObserver<T>>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Mapbox.Map.Map`1"/> class.
        /// </summary>
        /// <param name="fs"> The data source abstraction. </param>
        public Map(IFileSource fs)
        {
            this.fs = fs;
            this.latLngBounds = new GeoCoordinateBounds();
            this.zoom = 0;
        }

        /// <summary>
        ///     Gets or sets the tileset map ID. If not set, it will use the default
        ///     map ID for the tile type. I.e. "mapbox.satellite" for raster tiles
        ///     and "mapbox.mapbox-streets-v7" for vector tiles.
        /// </summary>
        /// <value> 
        ///     The tileset map ID, usually in the format "user.mapid". Exceptionally,
        ///     <see cref="T:Mapbox.Map.RasterTile"/> will take the full style URL
        ///     from where the tile is composited from, like "mapbox://styles/mapbox/streets-v9".
        /// </value>
        public string MapId
        {
            get
            {
                return this.mapId;
            }

            set
            {
                if (this.mapId == value)
                {
                    return;
                }

                this.mapId = value;

                foreach (Tile tile in this.tiles)
                {
                    tile.Cancel();
                }

                this.tiles.Clear();
                this.Update();
            }
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
        public int Zoom
        {
            get
            {
                return this.zoom;
            }

            set
            {
                this.zoom = Math.Max(0, Math.Min(20, value));
                this.Update();
            }
        }

        /// <summary>
        ///     Sets the coordinates bounds and zoom at once. More efficient than
        ///     doing it in two steps because it only causes one map update.
        /// </summary>
        /// <param name="bounds"> Coordinates bounds. </param>
        /// <param name="zoom"> Zoom level. </param>
        public void SetGeoCoordinateBoundsZoom(GeoCoordinateBounds bounds, int zoom)
        {
            this.latLngBounds = bounds;
            this.zoom = zoom;
            this.Update();
        }

        /// <summary> Add an <see cref="T:IObserver" /> to the observer list. </summary>
        /// <param name="observer"> The object subscribing to events. </param>
        public void Subscribe(Mapbox.IObserver<T> observer)
        {
            this.observers.Add(observer);
        }

        /// <summary> Remove an <see cref="T:IObserver" /> to the observer list. </summary>
        /// <param name="observer"> The object unsubscribing to events. </param>
        public void Unsubscribe(Mapbox.IObserver<T> observer)
        {
            this.observers.Remove(observer);
        }

        private void NotifyNext(T next)
        {
            var copy = new List<Mapbox.IObserver<T>>(this.observers);

            foreach (IObserver<T> observer in copy)
            {
                observer.OnNext(next);
            }
        }

        private void Update()
        {
            var cover = TileCover.Get(this.latLngBounds, this.zoom);

            if (cover.Count > TileMax)
            {
                return;
            }

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
                        this.NotifyNext(tile);

                        return true;
                    }
                });

            foreach (CanonicalTileId id in cover)
            {
                var tile = new T();

                Tile.Parameters param;
                param.Id = id;
                param.MapId = this.mapId;
                param.Fs = this.fs;

                tile.Initialize(param, () => { this.NotifyNext(tile); });

                this.tiles.Add(tile);
                this.NotifyNext(tile);
            }
        }
    }
}
