//-----------------------------------------------------------------------
// <copyright file="TileTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest {
	using Mapbox.Map;
	using NUnit.Framework;

	[TestFixture]
	internal class TileTest {
		private Mono.FileSource fs;

		[SetUp]
		public void SetUp() {
			this.fs = new Mono.FileSource();
		}

		[Test]
		[Ignore("Currently obsolete - we don't have that logic at the moment")]
		public void TileLoading() {
			byte[] data;

			var parameters = new Tile.Parameters();
			parameters.Fs = this.fs;
			parameters.Id = new CanonicalTileId(1, 1, 1);

			var tile = new RawPngRasterTile();
			tile.Initialize(parameters, () => { data = tile.Data; });


			Assert.Greater(tile.Data.Length, 1000);
		}

		[Test]
		[Ignore("Currently obsolete - we don't have that logic at the moment")]
		public void States() {
			var parameters = new Tile.Parameters();
			parameters.Fs = this.fs;
			parameters.Id = new CanonicalTileId(1, 1, 1);

			var tile = new RawPngRasterTile();
			Assert.AreEqual(Tile.State.New, tile.CurrentState);

			tile.Initialize(parameters, () => { });
			Assert.AreEqual(Tile.State.Loading, tile.CurrentState);

			Assert.AreEqual(Tile.State.Loaded, tile.CurrentState);

			tile.Cancel();
			Assert.AreEqual(Tile.State.Canceled, tile.CurrentState);
		}
	}
}
