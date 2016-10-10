//-----------------------------------------------------------------------
// <copyright file="FileSource.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Unity
{
    using System;
    using UnityEngine;

    /// <summary>
    ///     Unity implementation of the FileSource class. It will use Unity's
    ///     <see href="https://docs.unity3d.com/Manual/Coroutines.html">coroutines</see> to
    ///     asynchronously fetch data from the network via HTTP or HTTPS requests.
    /// </summary>
    /// <remarks>
    ///     A FileSource is needed for each MonoBehaviour in the game doing network requests.
    ///     The code is compatible for exporting to Unity's WebPlayer.
    /// </remarks>
    public sealed class FileSource : IFileSource
    {
        private readonly MonoBehaviour behaviour;
        private string accessToken;

        /// <summary> Initializes a new instance of the <see cref="FileSource" /> class. </summary>
        /// <param name="behaviour">
        ///     MonoBehaviour that will be used for processing the coroutine used on
        ///     the asynchronous request.
        /// </param>
        public FileSource(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
        }

        /// <summary> Performs a request asynchronously. </summary>
        /// <param name="url"> The HTTP/HTTPS url. </param>
        /// <param name="callback"> Callback to be called after the request is completed. </param>
        /// <returns>
        ///     Returns a <see cref="IAsyncRequest" /> that can be used for canceling a pending
        ///     request. This handle can be completely ignored if there is no intention of ever
        ///     canceling the request.
        /// </returns>
        public IAsyncRequest Request(string url, Action<Response> callback)
        {
            if (this.accessToken != null)
            {
                url += "?access_token=" + this.accessToken;
            }

            return new HTTPRequest(this.behaviour, url, callback);
        }

        /// <summary> Gets the access token. </summary>
        /// <returns> The access token. </returns>
        public string GetAccessToken()
        {
            return this.accessToken;
        }

        /// <summary>
        ///     Sets the <see hef="https://www.mapbox.com/help/define-access-token/">access token</see>
        ///     needed for accessing Mapbox services.
        /// </summary>
        /// <param name="token"> The access token. </param>
        public void SetAccessToken(string token)
        {
            this.accessToken = token;
        }
    }
}