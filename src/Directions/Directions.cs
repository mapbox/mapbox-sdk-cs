//-----------------------------------------------------------------------
// <copyright file="Directions.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox
{
    using System;
    using System.Text;

    /// <summary>
    ///     Wrapper around the <see href="https://www.mapbox.com/api-documentation/#directions">
    ///     Mapbox Directions API</see>. The Mapbox Directions API will show you how to get where
    ///     you're going.
    /// </summary>
    public sealed class Directions
    {
        private const string DirectionsAPI = Constants.BaseAPI + "directions/v5/";
        private readonly IFileSource fileSource;

        /// <summary> Initializes a new instance of the <see cref="Directions" /> class. </summary>
        /// <param name="fileSource"> Network access abstraction. </param>
        public Directions(IFileSource fileSource)
        {
            this.fileSource = fileSource;
        }

        /// <summary> Performs asynchronously a directions lookup. </summary>
        /// <param name="origin"> Geographic coordinates of the origin. </param>
        /// <param name="destination"> Geographic coordinates of the destination. </param>
        /// <param name="profile"> The routing <see cref="RoutingProfile"/>. </param>
        /// <param name="callback"> Callback to be called after the request is completed. </param>
        /// <returns>
        ///     Returns a <see cref="IAsyncRequest" /> that can be used for canceling a pending
        ///     request. This handle can be completely ignored if there is no intention of ever
        ///     canceling the request.
        /// </returns>
        public IAsyncRequest Query(LatLng origin, LatLng destination, RoutingProfile profile, Action<string> callback)
        {
            string url = DirectionsAPI + profile
                + origin.Longitude + "," + origin.Latitude + ";"
                + destination.Longitude + "," + destination.Latitude + ".json";

            return this.fileSource.Request(
                url,
                (Response response) =>
                {
                    // TODO: Parse the data.
                    callback(Encoding.UTF8.GetString(response.Data));
                });
        }
    }
}
