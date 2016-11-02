using UnityEngine;
using UnityEngine.UI;

using Mapbox;
using Mapbox.Geocoding;

public class SlippyGeocoder : MonoBehaviour
{
    private Mapbox.Unity.FileSource fs;
    private Geocoder geocoder;

    void Start()
    {
        fs = new Mapbox.Unity.FileSource(this);
        geocoder = new Geocoder(fs);
    }

    public void ForwardGeocoder()
    {
        var input = this.GetComponent<InputField>();
        var forwardGeocode = new ForwardGeocodeResource(input.text);

        var slippy = GameObject.Find("Slippy").GetComponent<Slippy>();
        fs.AccessToken = slippy.Token;

        geocoder.Geocode(forwardGeocode, (ForwardGeocodeResponse res) =>
            {
                if (res.Features.Count > 0)
                {
                    // FIXME: Geocoder coordinates are inverted.
                    slippy.SetCenter(new GeoCoordinate(
                        res.Features[0].Center.Longitude,
                        res.Features[0].Center.Latitude));
                }
            });
    }
}
