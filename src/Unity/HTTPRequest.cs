using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

namespace Mapbox.Unity
{
    sealed class HTTPRequest : IAsyncRequest
    {
        readonly UnityWebRequest request;
        readonly Action<Response> callback;

        public HTTPRequest(MonoBehaviour behaviour, string url, Action<Response> callback)
        {
            request = UnityWebRequest.Get(url);
            this.callback = callback;

            behaviour.StartCoroutine(doRequest());
        }

        public void Cancel()
        {
            request.Abort();
        }

        IEnumerator doRequest()
        {
            yield return request.Send();

            var response = new Response();
            response.error = request.error;
            response.data = request.downloadHandler.data;

            callback(response);
        }
    }
}