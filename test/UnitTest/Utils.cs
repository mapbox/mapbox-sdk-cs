//-----------------------------------------------------------------------
// <copyright file="Utils.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using Mapbox.Map;

    internal static class Utils
    {
        internal class VectorMapObserver : Mapbox.IObserver<VectorTile>
        {
            private List<VectorTile> tiles = new List<VectorTile>();

            public List<VectorTile> Tiles
            {
                get
                {
                    return tiles;
                }
            }

            public bool Complete { get; set; }

            public string Error { get; set; }

            public void OnCompleted()
            {
                Complete = true;
            }

            public void OnNext(VectorTile tile)
            {
                tiles.Add(tile);
            }

            public void OnError(string error)
            {
                Error = error;
            }
        }

        internal class RasterMapObserver : Mapbox.IObserver<RasterTile>
        {
            private List<Image> tiles = new List<Image>();

            public List<Image> Tiles
            {
                get
                {
                    return tiles;
                }
            }

            public bool Complete { get; set; }

            public string Error { get; set; }

            public void OnCompleted()
            {
                Complete = true;
            }

            public void OnNext(RasterTile tile)
            {
                if (tile.Error == null)
                {
                    var image = Image.FromStream(new MemoryStream(tile.Data));
                    tiles.Add(image);
                }
            }

            public void OnError(string error)
            {
                Error = error;
            }
        }

        internal class MockFileSource : IFileSource
        {
            private Dictionary<string, Response> responses = new Dictionary<string, Response>();
            private List<MockRequest> requests = new List<MockRequest>();

            public IAsyncRequest Request(string uri, Action<Response> callback)
            {
                var response = new Response();
                if (this.responses.ContainsKey(uri))
                {
                    response = this.responses[uri];
                }

                var request = new MockRequest(response, callback);
                this.requests.Add(request);

                return request;
            }

            public void SetReponse(string uri, Response response)
            {
                this.responses[uri] = response;
            }

            public void WaitForAllRequests()
            {
                while (this.requests.Count > 0)
                {
                    var req = this.requests[0];
                    this.requests.RemoveAt(0);

                    req.Run();
                }
            }

            public class MockRequest : IAsyncRequest
            {
                private Response response;
                private Action<Response> callback;

                public MockRequest(Response response, Action<Response> callback)
                {
                    this.response = response;
                    this.callback = callback;
                }

                public void Run()
                {
                    if (this.callback != null)
                    {
                        this.callback(this.response);
                        this.callback = null;
                    }
                }

                public void Cancel()
                {
                    this.callback = null;
                }
            }
        }
    }
}
