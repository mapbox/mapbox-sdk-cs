//-----------------------------------------------------------------------
// <copyright file="Map.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.IO;
	using System.ComponentModel;

	/// <summary>
	///     The Mapbox Map abstraction will take care of fetching and decoding
	///     data for a geographic bounding box at a certain zoom level.
	/// </summary>
	/// <typeparam name="T">
	///     The tile type, currently <see cref="T:Mapbox.Map.Vector"/> or
	///     <see cref="T:Mapbox.Map.Raster"/>.
	/// </typeparam>
	//TODO: if 'Map' changes from 'sealed' uncomment finalizer and change signature of 'Dispose(bool disposeManagedResources)'
	public sealed class Map<T> : IDisposable where T : Tile, new() {


		#region events


		/// <summary>
		/// Fires when a tile become available.
		/// </summary>
		public event EventHandler<MapTileReceivedEventArgs<T>> TileReceived;
		private void OnTileReceived(T tile) {
			if(_PauseTileUpdates) { return; }
			MapTileReceivedEventArgs<T> ea = new MapTileReceivedEventArgs<T>(tile);
			// Copy to a temporary variable to be thread-safe.
			EventHandler<MapTileReceivedEventArgs<T>> temp = TileReceived;
			if(null != temp) {
				temp(this, ea);
			}
		}


		/// <summary>
		/// Fires when all tiles for current map extent have been downloaded.
		/// </summary>
		public event EventHandler<EventArgs> QueueEmpty;
		private void OnQueueEmpty() {
			if(_PauseTileUpdates) { return; }
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EventArgs> temp = QueueEmpty;
			if(null != temp) {
				temp(this, EventArgs.Empty);
			}
		}


		#endregion


		private readonly SynchronizationContext syncContext;
		private bool _IsDisposed = false;
		private bool _PauseTileUpdates = false;
		private TileFetcher _TileFetcher;
		private GeoCoordinateBounds _LatLngBounds;
		private int _Zoom;
		private string _MapId;

		private HashSet<T> _Tiles = new HashSet<T>();
		//Lock for _Tiles during concurrent download
		private object _TilesLock = new object();

		/// <summary>
		///     Initializes a new instance of the <see cref="T:Mapbox.Map.Map`1"/> class.
		/// </summary>
		/// <param name="mainThreadId">Main thread id</param>
		/// <param name="fileSource"> The data source abstraction. </param>
		/// <param name="memoryTileCacheMin">Minimum number of tiles to cache in memory.</param>
		/// <param name="memoryTileCacheMax">Maximum number of tiles to cache in memory.</param>
		/// <param name="numberOfThreads">Size of threadpool for paralell tile fetching.</param>
		public Map(
			IFileSource fileSource
			, uint memoryTileCacheMin = 9
			, uint memoryTileCacheMax = 256
			, uint numberOfThreads = 4
		) {

			syncContext = AsyncOperationManager.SynchronizationContext;
			if(null == fileSource) {
				throw new ArgumentNullException("fileSource");
			}

			//HACK: sync downloading does not work at the moment.
			if(numberOfThreads < 2) {
				numberOfThreads = 2;
			}

			_LatLngBounds = new GeoCoordinateBounds();
			_Zoom = 0;

			_TileFetcher = new TileFetcher(
				fileSource
				, (int)memoryTileCacheMin
				, (int)memoryTileCacheMax
				, null
				, (int)numberOfThreads
			);
			_TileFetcher.TileReceived += TileFetcher_TileReceived;
			_TileFetcher.QueueEmpty += TileFetcher_QueueEmpty;
		}


		//TODO: uncomment if 'Map' class changes from 'sealed'
		//protected override void Dispose(bool disposeManagedResources)
		//~Map()
		//{
		//    Dispose(false);
		//}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		//TODO: change signature if 'Map' class changes from 'sealed'
		//protected override void Dispose(bool disposeManagedResources)
		public void Dispose(bool disposeManagedResources) {
			if(!_IsDisposed) {
				if(disposeManagedResources) {
					if(null != _TileFetcher) {
						_TileFetcher.TileReceived -= TileFetcher_TileReceived;
						_TileFetcher.QueueEmpty += TileFetcher_QueueEmpty;
						_TileFetcher.Clear();
						((IDisposable)_TileFetcher).Dispose();
						_TileFetcher = null;
					}
				}
			}
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
		public string MapId {
			get {
				return _MapId;
			}

			set {
				if(_MapId == value) {
					return;
				}

				_MapId = value;

				foreach(Tile tile in _Tiles) {
					tile.Cancel();
				}

				lock(_TilesLock) {
					_Tiles.Clear();
				}
				//abort download queue
				_TileFetcher.Clear();
				//clear volatile cache
				_TileFetcher.ClearMemoryCache();
				DownloadTiles();
			}
		}


		/// <summary>Gets or sets a geographic bounding box.</summary>
		/// <value>New geographic bounding box.</value>
		public GeoCoordinateBounds GeoCoordinateBounds {
			get {
				return _LatLngBounds;
			}

			set {
				_LatLngBounds = value;
				DownloadTiles();
			}
		}


		/// <summary>Gets or sets the central coordinate of the map.</summary>
		/// <value>The central coordinate.</value>
		public GeoCoordinate Center {
			get {
				return this._LatLngBounds.Center;
			}

			set {
				this._LatLngBounds.Center = value;
				this.DownloadTiles();
			}
		}


		/// <summary>Gets or sets the map zoom level.</summary>
		/// <value>The new zoom level.</value>
		public int Zoom {
			get {
				return this._Zoom;
			}

			set {
				this._Zoom = Math.Max(0, Math.Min(20, value));
				this.DownloadTiles();
			}
		}


		/// <summary>
		///     Sets the coordinates bounds and zoom at once. More efficient than
		///     doing it in two steps because it only causes one map update.
		/// </summary>
		/// <param name="bounds"> Coordinates bounds. </param>
		/// <param name="zoom"> Zoom level. </param>
		public void SetGeoCoordinateBoundsZoom(GeoCoordinateBounds bounds, int zoom) {
			this._LatLngBounds = bounds;
			this._Zoom = zoom;
			this.DownloadTiles();
		}


		/// <summary>
		/// Get HashSet of tile ids covering current extent
		/// </summary>
		/// <returns></returns>
		public HashSet<CanonicalTileId> GetTileCover() {
			return TileCover.Get(this._LatLngBounds, this._Zoom);
		}


		/// <summary>
		/// <para>Pause tile downloads.</para>
		/// <para>Useful when changing serveral map parameters to avoid unnecessary downloads.</para>
		/// <para>Use <see cref="EnableTileDownloading"/> when done changing map parameters.</para>
		/// </summary>
		public void DisableTileDownloading() { _PauseTileUpdates = true; }


		/// <summary>
		/// Resume tile downloads after <see cref="DisableTileDownloading"/>.
		/// </summary>
		public void EnableTileDownloading() { _PauseTileUpdates = false; }


		/// <summary>
		/// Abort current download queue.
		/// </summary>
		public void AbortDownloading() {
			if(null != _TileFetcher) {
				_TileFetcher.Clear();
			}
		}

		/// <summary>
		/// <para>Downloads tiles for current map extent.</para>
		/// <para>If <see cref="DisableTileDownloading"/> has been called before no tiles will be downloaded.</para>
		/// <para>Call <see cref="EnableTileDownloading"/> to enable downloading again.</para>
		/// </summary>
		public void DownloadTiles() {

			if(_PauseTileUpdates) { return; }

			var waitHandles = new List<WaitHandle>();
			var tilesNotImmediatelyAvailable = new List<CanonicalTileId>();

			HashSet<CanonicalTileId> tileCover = GetTileCover();

			foreach(var id in tileCover) {

				AutoResetEvent are = _TileFetcher.AsyncMode ? null : new AutoResetEvent(false);
				T tile = new T() { Id = id };
				byte[] tileData = _TileFetcher.GetTile(
					tile.MakeTileResource(_MapId).GetUrl()
					, id
					, are
				);

				if(null != tileData) {
					addTile(tileData, id, false, string.Empty);
				}

				if(are == null)
					continue;

				waitHandles.Add(are);
				tilesNotImmediatelyAvailable.Add(id);
			}

			//Wait for tiles
			foreach(var handle in waitHandles) {
				handle.WaitOne();
			}
		}


		private void TileFetcher_QueueEmpty(object sender, EventArgs e) {
			syncContext.Post(delegate { OnQueueEmpty(); }, null);
		}


		private void TileFetcher_TileReceived(object sender, TileFetcherTileReceivedEventArgs e) {
			addTile(e.Tile, e.TileId, e.HasError, e.ErrorMessage);
		}

		private void addTile(byte[] tileData, CanonicalTileId tileId, bool hasError, string errorMessage) {

			T tile = new T();
			tile.Id = tileId;

			//clone byte array to get rid of references
			//TODO: profile if this really helps
			if(null != tileData) {
				byte[] localTileData = null;
				using(MemoryStream ms = new MemoryStream(tileData)) {
					localTileData = ms.ToArray();
				}
				tile.ParseTileData(localTileData);
			}

			tile.SetState(Tile.State.Loaded);
			if(hasError) {
				tile.SetError(errorMessage);
			}
			lock(_TilesLock) {
				_Tiles.Add(tile);
			}
			syncContext.Post(delegate { OnTileReceived(tile); }, null);
		}


	}
}
