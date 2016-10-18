//-----------------------------------------------------------------------
// <copyright file="Map.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    /// <summary>
    ///     The Mapbox Map abstraction will take care of fetching and decoding
    ///     data for a geographic bounding box at a certain zoom level.
    /// </summary>
    public sealed class Map
    {
        private GeoCoordinateBounds latLngBounds;
        private double zoom;

        /// <summary> Initializes a new instance of the <see cref="Map" /> class. </summary>
        public Map()
        {
            this.GeoCoordinateBounds = new GeoCoordinateBounds();
            this.zoom = 0;
        }

        /// <summary>Gets or sets a geographic bounding box.</summary>
        /// <value>New geographic bounding box.</value>
        public GeoCoordinateBounds GeoCoordinateBounds
        {
            get
            {
                return this.latLngBounds;
            }

            set
            {
                this.latLngBounds = value;
                this.Update();
            }
        }

        /// <summary>Gets or sets the central coordinate of the map.</summary>
        /// <value>The central coordinate.</value>
        public GeoCoordinate Center
        {
            get
            {
                return this.latLngBounds.Center;
            }

            set
            {
                this.latLngBounds.Center = value;
                this.Update();
            }
        }

        /// <summary>Gets or sets the map zoom level.</summary>
        /// <value>The new zoom level.</value>
        public double Zoom
        {
            get
            {
                return this.zoom;
            }

            set
            {
                this.zoom = value;
                this.Update();
            }
        }

        private void Update()
        {
        }
    }
}
