//-----------------------------------------------------------------------
// <copyright file="TileCover.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Helper funtions to get a tile cover, i.e. a set of tiles needed for
    ///     covering a bounding box.
    /// </summary>
    public static class TileCover
    {
        /// <summary> Get a tile cover for the specified bounds and zoom. </summary>
        /// <param name="bounds"> Geographic bounding box.</param>
        /// <param name="zoom"> Zoom level. </param>
        /// <returns> The tile cover set. </returns>
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

        /// <summary> Converts a coordinate to a tile identifier. </summary>
        /// <param name="coord"> Geographic coordinate. </param>
        /// <param name="zoom"> Zoom level. </param>
        /// <returns>The to tile identifier.</returns>
        public static UnwrappedTileId CoordinateToTileId(GeoCoordinate coord, int zoom)
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
