//-----------------------------------------------------------------------
// <copyright file="MapTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using Mapbox.Map;
    using NUnit.Framework;

    [TestFixture]
    internal class MapTest
    {
        private Mono.FileSource fs;

        [SetUp]
        public void SetUp()
        {
            this.fs = new Mono.FileSource();
        }

        [Test]
        public void World()
        {
            var map = new Map<VectorTile>(this.fs);

            map.GeoCoordinateBounds = GeoCoordinateBounds.World();
            map.Zoom = 3;

            var mapObserver = new VectorMapObserver();
            map.Subscribe(mapObserver);

            this.fs.WaitForAllRequests();

            // TODO: Assert.True(mapObserver.Complete);
            // TODO: Assert.IsNull(mapObserver.Error);
            Assert.AreEqual(64, mapObserver.Tiles.Count);

            map.Unsubscribe(mapObserver);
        }

        [Test]
        public void Helsinki()
        {
            var map = new Map<RasterTile>(this.fs);

            map.Center = new GeoCoordinate(60.163200, 24.937700);
            map.Zoom = 13;

            var mapObserver = new RasterMapObserver();
            map.Subscribe(mapObserver);

            this.fs.WaitForAllRequests();

            // TODO: Assert.True(mapObserver.Complete);
            // TODO: Assert.IsNull(mapObserver.Error);
            Assert.AreEqual(1, mapObserver.Tiles.Count);
            Assert.AreEqual(new Size(256, 256), mapObserver.Tiles[0].Size);

            map.Unsubscribe(mapObserver);
        }

        [Test]
        public void ChangeSource()
        {
            var map = new Map<RasterTile>(this.fs);

            var mapObserver = new RasterMapObserver();
            map.Subscribe(mapObserver);

            map.Center = new GeoCoordinate(60.163200, 24.937700);
            map.Zoom = 13;
            map.Source = "invalid";

            this.fs.WaitForAllRequests();
            Assert.AreEqual(0, mapObserver.Tiles.Count);

            map.Source = "mapbox.terrain-rgb";

            this.fs.WaitForAllRequests();
            Assert.AreEqual(1, mapObserver.Tiles.Count);

            map.Source = null; // Use default source.

            this.fs.WaitForAllRequests();
            Assert.AreEqual(2, mapObserver.Tiles.Count);

            // Should have fetched tiles from different sources.
            Assert.AreNotEqual(mapObserver.Tiles[0], mapObserver.Tiles[1]);

            map.Unsubscribe(mapObserver);
        }
    }

    internal class VectorMapObserver : Mapbox.IObserver<VectorTile>
    {
        private List<VectorTile> tiles = new List<VectorTile>();

        public List<VectorTile> Tiles
        {
            get
            {
                return tiles;
            }
        }

        public bool Complete { get; set; }

        public string Error { get; set; }

        public void OnCompleted()
        {
            Complete = true;
        }

        public void OnNext(VectorTile tile)
        {
            tiles.Add(tile);
        }

        public void OnError(string error)
        {
            Error = error;
        }
    }

    internal class RasterMapObserver : Mapbox.IObserver<RasterTile>
    {
        private List<Image> tiles = new List<Image>();

        public List<Image> Tiles
        {
            get
            {
                return tiles;
            }
        }

        public bool Complete { get; set; }

        public string Error { get; set; }

        public void OnCompleted()
        {
            Complete = true;
        }

        public void OnNext(RasterTile tile)
        {
            if (tile.Error == null)
            {
                var image = Image.FromStream(new MemoryStream(tile.Data));
                tiles.Add(image);
            }
        }

        public void OnError(string error)
        {
            Error = error;
        }
    }
}
