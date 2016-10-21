//-----------------------------------------------------------------------
// <copyright file="MapTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System;
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

            this.fs.WaitForAllRequests();
        }

        [Test]
        public void Helsinki()
        {
            var map = new Map<VectorTile>(this.fs);

            map.Center = new GeoCoordinate(60.163200, 24.937700);
            map.Zoom = 13;

            this.fs.WaitForAllRequests();
        }
    }
}
