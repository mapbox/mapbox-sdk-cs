using System;
using System.Text;

using Mapbox.Platform;

namespace Mapbox
{
    public sealed class Geocoder
    {
        const string API = "https://api.mapbox.com/geocoding/v5/";
        readonly FileSource fileSource;

        public Geocoder(FileSource fileSource)
        {
            this.fileSource = fileSource;
        }

        public IAsyncRequest Reverse(LatLng coordinate, Action<string> callback)
        {
            const string mode = "mapbox.places/";
            string url = API + mode + coordinate.longitude + "," + coordinate.latitude + ".json";

            return fileSource.Request(url, (Response response) => {
                {
                    // TODO: Parse the data.
                    callback(Encoding.UTF8.GetString(response.data));
                }
            });
        }
    }
}