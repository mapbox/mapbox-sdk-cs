using System;
using System.Text;

namespace Mapbox
{
    public sealed class Geocoder
    {
        const string GeocoderAPI = Constants.BaseAPI + "geocoding/v5/";

        readonly IFileSource fileSource;

        public Geocoder(IFileSource fileSource)
        {
            this.fileSource = fileSource;
        }

        public IAsyncRequest Reverse(LatLng coordinate, Action<string> callback)
        {
            const string mode = "mapbox.places/";
            string url = GeocoderAPI + mode + coordinate.longitude + "," + coordinate.latitude + ".json";

            return fileSource.Request(url, (Response response) => {
                {
                    // TODO: Parse the data.
                    callback(Encoding.UTF8.GetString(response.data));
                }
            });
        }
    }
}