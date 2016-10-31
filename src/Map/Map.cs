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
    public sealed class Map<T> : IObservable<T> where T : Tile, new()
    {
        /// <summary>
        ///     Arbitrary limit of tiles this class will handle simultaneously.
        /// </summary>
        public const int TileMax = 256;

        private readonly IFileSource fs;
        private GeoCoordinateBounds latLngBounds;
        private double zoom;
        private string source;

        private HashSet<T> tiles = new HashSet<T>();
        private List<IObserver<T>> observers = new List<IObserver<T>>();

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
        ///     Gets or sets the tile source. If not set, it will use the default
        ///     source for the tile type. I.e. "mapbox.satellite" for raster tiles
        ///     and "mapbox.mapbox-streets-v7" for vector tiles.
        /// </summary>
        /// <value> The tile source. </value>
        public string Source
        {
            get
            {
                return this.source;
            }

            set
            {
                if (this.source == value)
                {
                    return;
                }

                this.source = value;

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
        public void Subscribe(IObserver<T> observer)
        {
            this.observers.Add(observer);
        }

        /// <summary> Remove an <see cref="T:IObserver" /> to the observer list. </summary>
        /// <param name="observer"> The object unsubscribing to events. </param>
        public void Unsubscribe(IObserver<T> observer)
        {
            this.observers.Remove(observer);
        }

        private void NotifyError(string error)
        {
            // Copying the list because you may unsubscribe when
            // notifying the observers.
            var copy = new List<IObserver<T>>(this.observers);

            foreach (IObserver<T> observer in copy)
            {
                observer.OnError(error);
            }
        }

        private void NotifyNext(T next)
        {
            var copy = new List<IObserver<T>>(this.observers);

            foreach (IObserver<T> observer in copy)
            {
                observer.OnNext(next);
            }
        }

        private void NotifyCompleted()
        {
            var copy = new List<IObserver<T>>(this.observers);

            foreach (IObserver<T> observer in copy)
            {
                observer.OnCompleted();
            }
        }

        private void Update()
        {
            var cover = TileCover.Get(this.latLngBounds, (int)Math.Ceiling(this.zoom));

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
                        return true;
                    }
                });

            foreach (CanonicalTileId id in cover)
            {
                var tile = new T();

                Tile.Parameters param;
                param.Id = id;
                param.Source = this.Source;
                param.Fs = this.fs;

                tile.Initialize(param, () => { this.NotifyNext(tile); });

                this.tiles.Add(tile);
            }
        }
    }
}
