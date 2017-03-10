//-----------------------------------------------------------------------
// <copyright file="VectorTileTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mapbox.Map;
    using Mapbox.Utils;
    using NUnit.Framework;

    [TestFixture]
    internal class VectorTileTest
    {
        private Mono.FileSource fs;

        [SetUp]
        public void SetUp()
        {
            this.fs = new Mono.FileSource();
        }

        [Test]
        public void ParseSuccess()
        {
            var map = new Map<VectorTile>(this.fs);

            var mapObserver = new Utils.VectorMapObserver();
            map.Subscribe(mapObserver);

            // Helsinki city center.
            map.Center = new GeoCoordinate(60.163200, 24.937700);

            for (int zoom = 0; zoom < 15; ++zoom)
            {
                map.Zoom = zoom;
                this.fs.WaitForAllRequests();
            }

            // We must have all the tiles for Helsinki from 0-15.
            Assert.AreEqual(15, mapObserver.Tiles.Count);

            foreach (var tile in mapObserver.Tiles)
            {
                Assert.Greater(tile.GeoJson.Length, 1000);
                Assert.Greater(tile.LayerNames().Count, 0, "Tile contains at least one layer");
                Mapbox.VectorTile.VectorTileLayer layer = tile.GetLayer("water");
                Assert.NotNull(layer, "Tile contains 'water' layer. Layers: {0}", string.Join(",", tile.LayerNames().ToArray()));
                Assert.Greater(layer.FeatureCount(), 0, "Water layer has features");
                Mapbox.VectorTile.VectorTileFeature feature = layer.GetFeature(0);
                Assert.Greater(feature.Geometry.Count, 0, "Feature has geometry");
            }

            map.Unsubscribe(mapObserver);
        }

        [Test]
        public void ParseFailure()
        {
            var resource = TileResource.MakeVector(new CanonicalTileId(13, 5465, 2371), null);

            var response = new Response();
            response.Data = Enumerable.Repeat((byte)0, 5000).ToArray();

            var mockFs = new Utils.MockFileSource();
            mockFs.SetReponse(resource.GetUrl(), response);

            var map = new Map<VectorTile>(mockFs);

            var mapObserver = new Utils.VectorMapObserver();
            map.Subscribe(mapObserver);

            map.Center = new GeoCoordinate(60.163200, 60.163200);
            map.Zoom = 13;

            mockFs.WaitForAllRequests();

            // TODO: Assert.AreEqual("Parse error.", mapObserver.Error);
            Assert.AreEqual(1, mapObserver.Tiles.Count);
            Assert.IsNull(mapObserver.Tiles[0].Data);

            map.Unsubscribe(mapObserver);
        }

        [Test]
        public void SeveralTiles()
        {
            var map = new Map<VectorTile>(this.fs);

            var mapObserver = new Utils.VectorMapObserver();
            map.Subscribe(mapObserver);

            map.GeoCoordinateBounds = GeoCoordinateBounds.World();
            map.Zoom = 3; // 64 tiles.

            this.fs.WaitForAllRequests();

            Assert.AreEqual(64, mapObserver.Tiles.Count);

            foreach (var tile in mapObserver.Tiles)
            {
                if (tile.Error == null)
                {
                    Assert.Greater(tile.GeoJson.Length, 41);
                }
                else
                {
                    // NotFound is fine.
                    Assert.AreNotEqual("ParseError", tile.Error);
                }
            }

            map.Unsubscribe(mapObserver);
        }
    }
}
