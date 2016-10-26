//-----------------------------------------------------------------------
// <copyright file="TileTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using Mapbox.Map;
    using NUnit.Framework;

    [TestFixture]
    internal class TileTest
    {
        private Mono.FileSource fs;

        [SetUp]
        public void SetUp()
        {
            this.fs = new Mono.FileSource();
        }

        [Test]
        public void TileLoading()
        {
            byte[] data;

            var parameters = new Tile.Parameters();
            parameters.Fs = this.fs;
            parameters.Id = new CanonicalTileId(1, 1, 1);

            var tile = new RawPngRasterTile();
            tile.Initialize(parameters, () => { data = tile.Data; });

            this.fs.WaitForAllRequests();

            Assert.Greater(tile.Data.Length, 1000);
        }
    }
}
