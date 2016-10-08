using System;

namespace Mapbox
{
    public interface IFileSource
    {
        IAsyncRequest Request(string url, Action<Response> callback);
    }
}