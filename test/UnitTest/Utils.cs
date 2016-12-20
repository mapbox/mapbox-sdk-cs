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

            public void OnNext(VectorTile tile)
            {
                if (tile.CurrentState == Tile.State.Loaded)
                {
                    tiles.Add(tile);
                }
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

            public void OnNext(RasterTile tile)
            {
                if (tile.CurrentState == Tile.State.Loaded && tile.Error == null)
                {
                    var image = Image.FromStream(new MemoryStream(tile.Data));
                    tiles.Add(image);
                }
            }
        }

        internal class ClassicRasterMapObserver : Mapbox.IObserver<ClassicRasterTile>
        {
            private List<Image> tiles = new List<Image>();

            public List<Image> Tiles
            {
                get
                {
                    return tiles;
                }
            }

            public void OnNext(ClassicRasterTile tile)
            {
                if (tile.CurrentState == Tile.State.Loaded && tile.Error == null)
                {
                    var image = Image.FromStream(new MemoryStream(tile.Data));
                    tiles.Add(image);
                }
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
