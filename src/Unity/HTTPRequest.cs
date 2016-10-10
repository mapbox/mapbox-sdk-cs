//-----------------------------------------------------------------------
// <copyright file="HTTPRequest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Unity
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Networking;

    internal sealed class HTTPRequest : IAsyncRequest
    {
        private readonly UnityWebRequest request;
        private readonly Action<Response> callback;

        public HTTPRequest(MonoBehaviour behaviour, string url, Action<Response> callback)
        {
            this.request = UnityWebRequest.Get(url);
            this.callback = callback;

            behaviour.StartCoroutine(this.DoRequest());
        }

        public void Cancel()
        {
            this.request.Abort();
        }

        private IEnumerator DoRequest()
        {
            yield return this.request.Send();

            var response = new Response();
            response.Error = this.request.error;
            response.Data = this.request.downloadHandler.data;

            this.callback(response);
        }
    }
}
