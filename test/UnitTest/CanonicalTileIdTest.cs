//-----------------------------------------------------------------------
// <copyright file="CanonicalTileIdTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
	using Mapbox.Map;
	using NUnit.Framework;

	[TestFixture]
	internal class CanonicalTileIdTest
	{
		[Test]
		public void ToGeoCoordinate()
		{
			var set = TileCover.Get(GeoCoordinateBounds.World(), 5);

			foreach (var tile in set)
			{
				var reverse = TileCover.CoordinateToTileId(tile.ToGeoCoordinate(), 5);

				Assert.AreEqual(tile.Z, reverse.Z);
				Assert.AreEqual(tile.X, reverse.X);
				Assert.AreEqual(tile.Y, reverse.Y);
			}
		}
	}
}