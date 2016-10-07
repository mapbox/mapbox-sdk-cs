using System;

namespace Mapbox.Platform
{
    public abstract class FileSource
    {
        protected string accessToken;

        public string GetAccessToken()
        {
            return accessToken;
        }

        public void SetAccessToken(string token)
        {
            accessToken = token;
        }

        public abstract IAsyncRequest Request(string url, Action<Response> callback);
    }
}