//-----------------------------------------------------------------------
// <copyright file="TileCover.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    using System;
    using System.Collections.Generic;

    internal static class TileCover
    {
        public static HashSet<CanonicalTileId> Get(GeoCoordinateBounds bounds, int zoom)
        {
            var tiles = new HashSet<CanonicalTileId>();

            if (bounds.IsEmpty() ||
                bounds.South > Constants.LatitudeMax ||
                bounds.North < -Constants.LatitudeMax)
            {
                return tiles;
            }

            var hull = GeoCoordinateBounds.FromCoordinates(
                new GeoCoordinate(Math.Max(bounds.South, -Constants.LatitudeMax), bounds.West),
                new GeoCoordinate(Math.Min(bounds.North, Constants.LatitudeMax), bounds.East));

            var sw = CoordinateToTileId(hull.SouthWest, zoom);
            var ne = CoordinateToTileId(hull.NorthEast, zoom);

            // Scanlines.
            for (var x = sw.X; x <= ne.X; ++x)
            {
                for (var y = ne.Y; y <= sw.Y; ++y)
                {
                    tiles.Add(new UnwrappedTileId(zoom, x, y).Canonical);
                }
            }

            return tiles;
        }

        private static UnwrappedTileId CoordinateToTileId(GeoCoordinate coord, int zoom)
        {
            var lat = coord.Latitude;
            var lng = coord.Longitude;

            // See: http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
            var x = (int)Math.Floor((lng + 180.0) / 360.0 * Math.Pow(2.0, zoom));
            var y = (int)Math.Floor((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0)
                    + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * Math.Pow(2.0, zoom));

            return new UnwrappedTileId(zoom, x, y);
        }
    }
}
