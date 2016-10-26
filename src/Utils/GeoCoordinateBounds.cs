//-----------------------------------------------------------------------
// <copyright file="GeoCoordinateBounds.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox
{
    /// <summary> Represents a bounding box derived from a southwest corner and a northeast corner. </summary>
    public struct GeoCoordinateBounds
    {
        /// <summary> Southwest corner of bounding box. </summary>
        public GeoCoordinate SouthWest;

        /// <summary> Northeast corner of bounding box. </summary>
        public GeoCoordinate NorthEast;

        /// <summary> Initializes a new instance of the <see cref="GeoCoordinateBounds" /> struct. </summary>
        /// <param name="sw"> Geographic coordinate representing southwest corner of bounding box. </param>
        /// <param name="ne"> Geographic coordinate representing northeast corner of bounding box. </param>
        public GeoCoordinateBounds(GeoCoordinate sw, GeoCoordinate ne)
        {
            this.SouthWest = sw;
            this.NorthEast = ne;
        }

        /// <summary> Gets the south latitude. </summary>
        /// <value> The south latitude. </value>
        public double South
        {
            get
            {
                return this.SouthWest.Latitude;
            }
        }

        /// <summary> Gets the west longitude. </summary>
        /// <value> The west longitude. </value>
        public double West
        {
            get
            {
                return this.SouthWest.Longitude;
            }
        }

        /// <summary> Gets the north latitude. </summary>
        /// <value> The north latitude. </value>
        public double North
        {
            get
            {
                return this.NorthEast.Latitude;
            }
        }

        /// <summary> Gets the east longitude. </summary>
        /// <value> The east longitude. </value>
        public double East
        {
            get
            {
                return this.NorthEast.Longitude;
            }
        }

        /// <summary>
        ///     Gets or sets the central coordinate of the bounding box. When
        ///     setting a new center, the bounding box will retain its original size.
        /// </summary>
        /// <value> The central coordinate. </value>
        public GeoCoordinate Center
        {
            get
            {
                var lat = (this.SouthWest.Latitude + this.NorthEast.Latitude) / 2;
                var lng = (this.SouthWest.Longitude + this.NorthEast.Longitude) / 2;

                return new GeoCoordinate(lat, lng);
            }

            set
            {
                var lat = (this.NorthEast.Latitude - this.SouthWest.Latitude) / 2;
                this.SouthWest.Latitude = value.Latitude - lat;
                this.NorthEast.Latitude = value.Latitude + lat;

                var lng = (this.NorthEast.Longitude - this.SouthWest.Longitude) / 2;
                this.SouthWest.Longitude = value.Longitude - lng;
                this.NorthEast.Longitude = value.Longitude + lng;
            }
        }

        /// <summary>
        ///     Creates a bound from two arbitrary points. Contrary to the constructor,
        ///     this method always creates a non-empty box.
        /// </summary>
        /// <param name="a"> The first point. </param>
        /// <param name="b"> The second point. </param>
        /// <returns> The convex hull. </returns>
        public static GeoCoordinateBounds FromCoordinates(GeoCoordinate a, GeoCoordinate b)
        {
            var bounds = new GeoCoordinateBounds(a, a);
            bounds.Extend(b);

            return bounds;
        }

        /// <summary> A bounding box containing the world. </summary>
        /// <returns> The world bounding box. </returns>
        public static GeoCoordinateBounds World()
        {
            var sw = new GeoCoordinate(-90, -180);
            var ne = new GeoCoordinate(90, 180);

            return new GeoCoordinateBounds(sw, ne);
        }

        /// <summary> Extend the bounding box to contain the point. </summary>
        /// <param name="point"> A geographic coordinate. </param>
        public void Extend(GeoCoordinate point)
        {
            if (point.Latitude < this.SouthWest.Latitude)
            {
                this.SouthWest.Latitude = point.Latitude;
            }

            if (point.Latitude > this.NorthEast.Latitude)
            {
                this.NorthEast.Latitude = point.Latitude;
            }

            if (point.Longitude < this.SouthWest.Longitude)
            {
                this.SouthWest.Longitude = point.Longitude;
            }

            if (point.Longitude > this.NorthEast.Longitude)
            {
                this.NorthEast.Longitude = point.Longitude;
            }
        }

        /// <summary> Extend the bounding box to contain the bounding box. </summary>
        /// <param name="bounds"> A bounding box. </param>
        public void Extend(GeoCoordinateBounds bounds)
        {
            this.Extend(bounds.SouthWest);
            this.Extend(bounds.NorthEast);
        }

        /// <summary> Whenever the geographic bounding box is empty. </summary>
        /// <returns> <c>true</c>, if empty, <c>false</c> otherwise. </returns>
        public bool IsEmpty()
        {
            return this.SouthWest.Latitude > this.NorthEast.Latitude ||
                       this.SouthWest.Longitude > this.NorthEast.Longitude;
        }

        /// <summary>
        /// Converts to an array of doubles.
        /// </summary>
        /// <returns>An array of coordinates.</returns>
        public double[] ToArray()
        {
            double[] array =
            {
                this.SouthWest.Latitude,
                this.SouthWest.Longitude,
                this.NorthEast.Latitude,
                this.NorthEast.Longitude
            };

            return array;
        }

        /// <summary> Converts the Bbox to a URL snippet. </summary>
        /// <returns> Returns a string for use in a Mapbox query URL. </returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", this.SouthWest.ToString(), this.NorthEast.ToString());
        }
    }
}
