//-----------------------------------------------------------------------
// <copyright file="MapTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System;
    using System.Collections.Generic;
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

            var mapObserver = new MapObserver();
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
            var map = new Map<VectorTile>(this.fs);

            map.Center = new GeoCoordinate(60.163200, 24.937700);
            map.Zoom = 13;

            var mapObserver = new MapObserver();
            map.Subscribe(mapObserver);

            this.fs.WaitForAllRequests();

            // TODO: Assert.True(mapObserver.Complete);
            // TODO: Assert.IsNull(mapObserver.Error);
            Assert.AreEqual(1, mapObserver.Tiles.Count);

            map.Unsubscribe(mapObserver);
        }
    }

    internal class MapObserver : Mapbox.IObserver<VectorTile>
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
}
