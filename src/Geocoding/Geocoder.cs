namespace Mapbox
{
    public class Geocoder
    {
        readonly FileSource fs = new FileSource();

        public LatLng Forward()
        {
            return new LatLng(fs.Request(), 10);
        }
    }
}