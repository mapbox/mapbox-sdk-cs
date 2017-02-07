//-----------------------------------------------------------------------
// <copyright file="VectorTileTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest {


	using System.Linq;
	using Map;
	using NUnit.Framework;


	[TestFixture]
	internal class VectorTileTest {


		private Mono.FileSource fs;
		private bool _TileLoadingFinished;
		private System.Collections.Generic.List<Tile> _Tiles;
		private System.Collections.Generic.List<Tile> _FailedTiles;


		private void Map_QueueEmpty(object sender, System.EventArgs e) {
			_TileLoadingFinished = true;
		}
		private void MapVector_TileReceived(object sender, MapTileReceivedEventArgs<VectorTile> e) {
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
		public void ParseSuccess() {

			var map = new Map<VectorTile>(
				this.fs
				, 15
				, 16
				, 4
			);

			//Pause tile fetching when multiple parameters are changed
			map.DisableTileDownloading();

			map.TileReceived += MapVector_TileReceived;
			map.QueueEmpty += Map_QueueEmpty;

			_Tiles = new System.Collections.Generic.List<Tile>();
			_FailedTiles = new System.Collections.Generic.List<Tile>();
			_TileLoadingFinished = false;

			// Helsinki city center.
			map.Center = new GeoCoordinate(60.163200, 24.937700);

			map.EnableTileDownloading();

			for(int zoom = 0; zoom < 15; ++zoom) {
				_TileLoadingFinished = false;
				map.Zoom = zoom;
				//wait for all requests
				while(!_TileLoadingFinished) {
					System.Threading.Thread.Sleep(5);
				}
			}

			// We must have all the tiles for Helsinki from 0-15.
			Assert.AreEqual(15, _Tiles.Count);

			foreach(var tile in _Tiles) {
				VectorTile vt = tile as VectorTile;
				Assert.Greater(vt.GeoJson.Length, 1000);
				Assert.Greater(vt.LayerNames().Count, 0, "Tile contains at least one layer");
				Mapbox.VectorTile.VectorTileLayer layer = vt.GetLayer("water");
				Assert.NotNull(layer, "Tile contains 'water' layer. Layers: {0}", string.Join(",", vt.LayerNames().ToArray()));
				Assert.Greater(layer.FeatureCount(), 0, "Water layer has features");
				Mapbox.VectorTile.VectorTileFeature feature = layer.GetFeature(0);
				Assert.Greater(feature.Geometry.Count, 0, "Feature has geometry");
			}

			map.TileReceived -= MapVector_TileReceived;
			map.QueueEmpty -= Map_QueueEmpty;
			map.Dispose();
			map = null;
		}


		[Test, Timeout(8000)]
		public void ParseFailure() {

			var resource = TileResource.MakeVector(new CanonicalTileId(13, 5465, 2371), null);

			var response = new Response();
			response.Data = Enumerable.Repeat((byte)0, 5000).ToArray();

			var mockFs = new Utils.MockFileSource();
			mockFs.SetReponse(resource.GetUrl(), response);

			var map = new Map<VectorTile>(
				mockFs
				, 1
				, 2
				, 4
			);

			//Pause tile fetching when multiple parameters are changed
			map.DisableTileDownloading();

			map.TileReceived += MapVector_TileReceived;
			map.QueueEmpty += Map_QueueEmpty;

			_Tiles = new System.Collections.Generic.List<Tile>();
			_FailedTiles = new System.Collections.Generic.List<Tile>();
			_TileLoadingFinished = false;

			map.Center = new GeoCoordinate(60.163200, 60.163200);

			map.EnableTileDownloading();

			map.Zoom = 13;

			//wait for all requests
			while(!_TileLoadingFinished) {
				System.Threading.Thread.Sleep(5);
			}

			Assert.AreEqual(1, _FailedTiles.Count);
			Assert.IsNull(((VectorTile)_FailedTiles[0]).Data);

			map.TileReceived -= MapVector_TileReceived;
			map.QueueEmpty -= Map_QueueEmpty;
			map.Dispose();
			map = null;
		}


		[Test, Timeout(8000)]
		public void SeveralTiles() {

			var map = new Map<VectorTile>(
				this.fs
				, 64
				, 65
				, 4
			);

			//Pause tile fetching when multiple parameters are changed
			map.DisableTileDownloading();

			map.TileReceived += MapVector_TileReceived;
			map.QueueEmpty += Map_QueueEmpty;

			_Tiles = new System.Collections.Generic.List<Tile>();
			_FailedTiles = new System.Collections.Generic.List<Tile>();
			_TileLoadingFinished = false;

			map.GeoCoordinateBounds = GeoCoordinateBounds.World();

			map.EnableTileDownloading();

			map.Zoom = 3; // 64 tiles.

			while(!_TileLoadingFinished) {
				System.Threading.Thread.Sleep(5);
			}

			Assert.AreEqual(61, _Tiles.Count);
			//TODO: 3 tiles from Antartic seem to be missing
			//missing tiles: 3/5/7, 3/6/7, 3/7/7
			Assert.AreEqual(3, _FailedTiles.Count);

			foreach(var tile in _Tiles) {
				VectorTile vt = (VectorTile)tile;
				if(tile.Error == null) {
					Assert.Greater(vt.GeoJson.Length, 41);
				} else {
					// NotFound is fine.
					Assert.AreNotEqual("ParseError", tile.Error);
				}
			}

			map.TileReceived -= MapVector_TileReceived;
			map.QueueEmpty -= Map_QueueEmpty;
			map.Dispose();
			map = null;
		}


	}
}
