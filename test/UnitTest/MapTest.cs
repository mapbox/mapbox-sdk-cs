//-----------------------------------------------------------------------
// <copyright file="MapTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest {
	using System.Drawing;
	using Mapbox.Map;
	using NUnit.Framework;
	using System.Threading;

	[TestFixture]
	internal class MapTest {


		private Mono.FileSource fs;
		private bool _TileLoadingFinished;
		private System.Collections.Generic.List<Tile> _Tiles;
		private object _LockTiles = new object();
		private System.Collections.Generic.List<Tile> _FailedTiles;
		private object _LockFailedTiles = new object();


		private void Map_QueueEmpty(object sender, System.EventArgs e) {
			_TileLoadingFinished = true;
		}
		private void MapVector_TileReceived(object sender, MapTileReceivedEventArgs<VectorTile> e) {
			//System.Diagnostics.Debug.WriteLine("Map_TileReceived: {0}", e.Tile.Id);
			if(!string.IsNullOrWhiteSpace(e.Tile.Error)) {
				lock(_LockFailedTiles) { _FailedTiles.Add(e.Tile); }
			} else {
				lock(_LockTiles) { _Tiles.Add(e.Tile); }
			}
		}
		private void MapRaster_TileReceived(object sender, MapTileReceivedEventArgs<RasterTile> e) {
			//System.Diagnostics.Debug.WriteLine("Map_TileReceived: {0}", e.Tile.Id);
			if(!string.IsNullOrWhiteSpace(e.Tile.Error)) {
				lock(_LockFailedTiles) { _FailedTiles.Add(e.Tile); }
			} else {
				lock(_LockTiles) { _Tiles.Add(e.Tile); }
			}
		}
		private void MapClassicRaster_TileReceived(object sender, MapTileReceivedEventArgs<ClassicRasterTile> e) {
			//System.Diagnostics.Debug.WriteLine("Map_TileReceived: {0}", e.Tile.Id);
			if(!string.IsNullOrWhiteSpace(e.Tile.Error)) {
				_FailedTiles.Add(e.Tile);
			} else {
				_Tiles.Add(e.Tile);
			}
		}


		[SetUp]
		public void SetUp() {
			this.fs = new Mono.FileSource();
		}


		[Test, Timeout(16000)]
		public void World() {

			var map = new Map<VectorTile>(
				this.fs
				, 64
				, 65
				, 4
			);

			//Pause tile fetching when multiple parameters are changed
			map.DisableTileDownloading();
			map.GeoCoordinateBounds = GeoCoordinateBounds.World();
			map.Zoom = 3;

			map.TileReceived += MapVector_TileReceived;
			map.QueueEmpty += Map_QueueEmpty;

			_Tiles = new System.Collections.Generic.List<Tile>();
			_FailedTiles = new System.Collections.Generic.List<Tile>();
			_TileLoadingFinished = false;

			map.EnableTileDownloading();
			map.DownloadTiles();

			//wait for all requests
			while(!_TileLoadingFinished) {
				System.Threading.Thread.Sleep(5);
			}

			Assert.AreEqual(61, _Tiles.Count);
			//TODO: 3 tiles from Antartic seem to be missing
			//missing tiles: 3/5/7, 3/6/7, 3/7/7
			Assert.AreEqual(3, _FailedTiles.Count);

			map.TileReceived -= MapVector_TileReceived;
			map.QueueEmpty -= Map_QueueEmpty;
			map.Dispose();
			map = null;
		}


		[Test, Timeout(8000)]
		public void RasterHelsinki() {

			var map = new Map<RasterTile>(
				this.fs
				, 64
				, 65
				, 4
			);

			map.DisableTileDownloading();
			map.Center = new GeoCoordinate(60.163200, 24.937700);
			map.Zoom = 13;

			map.TileReceived += MapRaster_TileReceived;
			map.QueueEmpty += Map_QueueEmpty;

			_Tiles = new System.Collections.Generic.List<Tile>();
			_FailedTiles = new System.Collections.Generic.List<Tile>();
			_TileLoadingFinished = false;

			map.EnableTileDownloading();
			map.DownloadTiles();

			//wait for all requests
			while(!_TileLoadingFinished) {
				System.Threading.Thread.Sleep(5);
			}

			Assert.AreEqual(1, _Tiles.Count);
			var image = Image.FromStream(new System.IO.MemoryStream(((RasterTile)_Tiles[0]).Data));
			Assert.AreEqual(new Size(512, 512), image.Size);

			map.TileReceived -= MapRaster_TileReceived;
			map.QueueEmpty -= Map_QueueEmpty;
			map.Dispose();
			map = null;
		}


		[Test, Timeout(8000)]
		public void ChangeMapId() {

			var map = new Map<ClassicRasterTile>(
				this.fs
				, 64
				, 65
				, 4
			);

			map.DisableTileDownloading();

			map.TileReceived += MapClassicRaster_TileReceived;
			map.QueueEmpty += Map_QueueEmpty;

			map.Center = new GeoCoordinate(60.163200, 24.937700);
			map.Zoom = 13;
			map.MapId = "invalid";

			_FailedTiles = new System.Collections.Generic.List<Tile>();
			_Tiles = new System.Collections.Generic.List<Tile>();

			map.EnableTileDownloading();
			map.DownloadTiles();

			//wait for all requests
			while(!_TileLoadingFinished) {
				System.Threading.Thread.Sleep(5);
			}
			Assert.AreEqual(1, _FailedTiles.Count);
			Assert.AreEqual(0, _Tiles.Count);

			_TileLoadingFinished = false;
			_FailedTiles = new System.Collections.Generic.List<Tile>();
			_Tiles = new System.Collections.Generic.List<Tile>();

			map.MapId = "mapbox.terrain-rgb";

			//wait for all requests
			while(!_TileLoadingFinished) {
				System.Threading.Thread.Sleep(5);
			}

			Assert.AreEqual(0, _FailedTiles.Count);
			Assert.AreEqual(1, _Tiles.Count);

			_TileLoadingFinished = false;
			_FailedTiles = new System.Collections.Generic.List<Tile>();
			_Tiles = new System.Collections.Generic.List<Tile>();

			map.MapId = null; // Use default map ID.

			//wait for all requests
			while(!_TileLoadingFinished) {
				System.Threading.Thread.Sleep(5);
			}

			Assert.AreEqual(0, _FailedTiles.Count);
			Assert.AreEqual(1, _Tiles.Count);

			map.TileReceived -= MapClassicRaster_TileReceived;
			map.QueueEmpty -= Map_QueueEmpty;
			map.Dispose();
			map = null;
		}


		[Test, Timeout(8000)]
		public void Zoom() {
			var map = new Map<RasterTile>(
				this.fs
				, 64
				, 65
				, 4
			);

			map.Zoom = 50;
			Assert.AreEqual(20, map.Zoom);

			map.Zoom = -50;
			Assert.AreEqual(0, map.Zoom);
		}


	}
}
