//-----------------------------------------------------------------------
// <copyright file="TileResourceTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using Mapbox.Map;
    using NUnit.Framework;

    [TestFixture]
    internal class TileResourceTest
    {
        private string api;
        private CanonicalTileId id;

        [SetUp]
        public void SetUp()
        {
            this.api = "https://api.mapbox.com/v4/";
            this.id = new CanonicalTileId(0, 0, 0);
        }

        [Test]
        public void GetUrlRaster()
        {
            var res1 = TileResource.MakeRaster(this.id, null);
            Assert.AreEqual(this.api + "mapbox.satellite/0/0/0.png", res1.GetUrl());

            var res2 = TileResource.MakeRaster(this.id, "foobar");
            Assert.AreEqual(this.api + "foobar/0/0/0.png", res2.GetUrl());

            var res3 = TileResource.MakeRaster(this.id, "test");
            Assert.AreEqual(this.api + "test/0/0/0.png", res3.GetUrl());
        }

        [Test]
        public void GetUrlVector()
        {
            var res1 = TileResource.MakeVector(this.id, null);
            Assert.AreEqual(this.api + "mapbox.mapbox-streets-v7/0/0/0.vector.pbf", res1.GetUrl());

            var res2 = TileResource.MakeVector(this.id, "foobar");
            Assert.AreEqual(this.api + "foobar/0/0/0.vector.pbf", res2.GetUrl());

            var res3 = TileResource.MakeVector(this.id, "test");
            Assert.AreEqual(this.api + "test/0/0/0.vector.pbf", res3.GetUrl());
        }
    }
}