//-----------------------------------------------------------------------
// <copyright file="GeoCoordinateBoundsTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System;
    using Mapbox;
    using NUnit.Framework;

    [TestFixture]
    internal class GeoCoordinateBoundsTest
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void SmallBounds()
        {
            var a = new GeoCoordinate(0, 0);
            var b = new GeoCoordinate(10, 10);
            var bounds = new GeoCoordinateBounds(a, b);
            Assert.AreEqual("0.00000,0.00000,10.00000,10.00000", bounds.ToString());
        }
    }
}