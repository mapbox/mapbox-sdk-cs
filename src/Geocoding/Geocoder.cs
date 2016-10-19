//-----------------------------------------------------------------------
// <copyright file="Geocoder.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Geocoding
{
    using System;
    using System.Text;

    /// <summary>
    ///     Wrapper around the <see href="https://www.mapbox.com/api-documentation/#geocoding">
    ///     Mapbox Geocoding API</see>. The Geocoder does two things: geocoding and reverse geocoding.
    /// </summary>
    public sealed class Geocoder
    {
        private readonly IFileSource fileSource;

        /// <summary> Initializes a new instance of the <see cref="Geocoder" /> class. </summary>
        /// <param name="fileSource"> Network access abstraction. </param>
        public Geocoder(IFileSource fileSource)
        {
            this.fileSource = fileSource;
        }

        /// <summary> Performs asynchronously a forward geocoding lookup. </summary>
        /// <param name="geocode"> Geocode resource. </param>
        /// <param name="callback"> Callback to be called after the request is completed. </param>
        /// <returns>
        ///     Returns a <see cref="IAsyncRequest" /> that can be used for canceling a pending
        ///     request. This handle can be completely ignored if there is no intention of ever
        ///     canceling the request.
        /// </returns>
        public IAsyncRequest Forward(ForwardGeocodeResource geocode, Action<string> callback)
        {
            return this.fileSource.Request(
                geocode.GetUrl(),
                (Response response) =>
                {
                    // TODO: Parse the data.
                    callback(Encoding.UTF8.GetString(response.Data));
                });
        }

        /// <summary> Performs asynchronously a reverse geocoding lookup. </summary>
        /// <param name="geocode"> Geocode resource. </param>
        /// <param name="callback"> Callback to be called after the request is completed. </param>
        /// <returns>
        ///     Returns a <see cref="IAsyncRequest" /> that can be used for canceling a pending
        ///     request. This handle can be completely ignored if there is no intention of ever
        ///     canceling the request.
        /// </returns>
        public IAsyncRequest Reverse(ReverseGeocodeResource geocode, Action<string> callback)
        {
            return this.fileSource.Request(
                geocode.GetUrl(),
                (Response response) =>
                {
                    // TODO: Parse the data.
                    callback(Encoding.UTF8.GetString(response.Data));
                });
        }
    }
}