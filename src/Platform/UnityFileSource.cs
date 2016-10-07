using System;

using UnityEngine;

namespace Mapbox.Platform.Unity
{
    sealed public class UnityFileSource : FileSource
    {
        readonly MonoBehaviour behaviour;

        public UnityFileSource(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
        }

        public override IAsyncRequest Request(string url, Action<Response> callback)
        {
            if (accessToken != null)
            {
                url += "?access_token=" + accessToken;
            }

            return new UnityHTTPRequest(behaviour, url, callback);
        }

    }
}