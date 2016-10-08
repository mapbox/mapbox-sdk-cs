using System;

using UnityEngine;

namespace Mapbox.Unity
{
    sealed public class FileSource : IFileSource
    {
        string accessToken;
        readonly MonoBehaviour behaviour;

        public FileSource(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
        }

        public IAsyncRequest Request(string url, Action<Response> callback)
        {
            if (accessToken != null)
            {
                url += "?access_token=" + accessToken;
            }

            return new HTTPRequest(behaviour, url, callback);
        }

        public string GetAccessToken()
        {
            return accessToken;
        }

        public void SetAccessToken(string token)
        {
            accessToken = token;
        }
    }
}