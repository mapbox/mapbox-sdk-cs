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

    internal sealed class HTTPRequestWWW : IAsyncRequest
    {
        private WWW request;
        private readonly Action<Response> callback;

        public HTTPRequestWWW(MonoBehaviour behaviour, string url, Action<Response> callback)
        {
            this.callback = callback;

            behaviour.StartCoroutine(this.DoRequest(url));
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        private IEnumerator DoRequest(string url)
        {
            request = new WWW(url);
            yield return request;

            var response = new Response();
            response.Headers = request.responseHeaders;
            response.Error = request.error;
            response.Data = request.bytes;

            //http://answers.unity3d.com/questions/474421/wwwtexture-dispose-didnt-work-causing-memory-leak.html
            request.Dispose();
            request = null;

            callback(response);
        }
    }
}
