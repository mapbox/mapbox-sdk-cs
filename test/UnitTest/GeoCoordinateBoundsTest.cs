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

		[Test]
		public void Extend()
		{
			var bounds1 = new GeoCoordinateBounds(new GeoCoordinate(-10, -10), new GeoCoordinate(10, 10));
			var bounds2 = new GeoCoordinateBounds(new GeoCoordinate(-20, -20), new GeoCoordinate(20, 20));

			bounds1.Extend(bounds2);

			Assert.AreEqual(bounds1.South, bounds2.South);
			Assert.AreEqual(bounds1.West, bounds2.West);
			Assert.AreEqual(bounds1.North, bounds2.North);
			Assert.AreEqual(bounds1.East, bounds2.East);
		}

		[Test]
		public void Hull()
		{
			var bounds1 = new GeoCoordinateBounds(new GeoCoordinate(-10, -10), new GeoCoordinate(10, 10));
			var bounds2 = GeoCoordinateBounds.FromCoordinates(new GeoCoordinate(10, 10), new GeoCoordinate(-10, -10));

			Assert.AreEqual(bounds1.South, bounds2.South);
			Assert.AreEqual(bounds1.West, bounds2.West);
			Assert.AreEqual(bounds1.North, bounds2.North);
			Assert.AreEqual(bounds1.East, bounds2.East);
		}

		[Test]
		public void World()
		{
			var bounds = GeoCoordinateBounds.World();

			Assert.AreEqual(bounds.South, -90);
			Assert.AreEqual(bounds.West, -180);
			Assert.AreEqual(bounds.North, 90);
			Assert.AreEqual(bounds.East, 180);
		}

		[Test]
		public void CardinalLimits()
		{
			var bounds = new GeoCoordinateBounds(new GeoCoordinate(10, 20), new GeoCoordinate(30, 40));

			// SouthWest, first parameter.
			Assert.AreEqual(bounds.South, 10);
			Assert.AreEqual(bounds.West, 20);

			// NorthEast, second parameter.
			Assert.AreEqual(bounds.North, 30);
			Assert.AreEqual(bounds.East, 40);
		}

		[Test]
		public void IsEmpty()
		{
			var bounds1 = new GeoCoordinateBounds(new GeoCoordinate(10, 10), new GeoCoordinate(0, 0));
			Assert.IsTrue(bounds1.IsEmpty());

			var bounds2 = new GeoCoordinateBounds(new GeoCoordinate(0, 0), new GeoCoordinate(0, 0));
			Assert.IsFalse(bounds2.IsEmpty());

			var bounds3 = new GeoCoordinateBounds(new GeoCoordinate(0, 0), new GeoCoordinate(10, 10));
			Assert.IsFalse(bounds3.IsEmpty());
		}

		[Test]
		public void Center()
		{
			var bounds1 = new GeoCoordinateBounds(new GeoCoordinate(0, 0), new GeoCoordinate(0, 0));
			Assert.AreEqual(bounds1.Center, new GeoCoordinate(0, 0));

			bounds1.Center = new GeoCoordinate(10, 10);
			Assert.AreEqual(new GeoCoordinateBounds(new GeoCoordinate(10, 10), new GeoCoordinate(10, 10)), bounds1);

			var bounds2 = new GeoCoordinateBounds(new GeoCoordinate(-10, -10), new GeoCoordinate(10, 10));
			Assert.AreEqual(bounds2.Center, new GeoCoordinate(0, 0));

			bounds2.Center = new GeoCoordinate(10, 10);
			Assert.AreEqual(new GeoCoordinateBounds(new GeoCoordinate(0, 0), new GeoCoordinate(20, 20)), bounds2);

			var bounds3 = new GeoCoordinateBounds(new GeoCoordinate(0, 0), new GeoCoordinate(20, 40));
			Assert.AreEqual(bounds3.Center, new GeoCoordinate(10, 20));

			bounds3.Center = new GeoCoordinate(10, 10);
			Assert.AreEqual(new GeoCoordinateBounds(new GeoCoordinate(0, -10), new GeoCoordinate(20, 30)), bounds3);
		}
	}
}
