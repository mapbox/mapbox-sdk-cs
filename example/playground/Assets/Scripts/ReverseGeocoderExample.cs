using Mapbox;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class ReverseGeocoderExample : MonoBehaviour
{
	[SerializeField]
	ReverseGeocodeUserInput _searchLocation;

	[SerializeField]
	Text _resultsText;

	void Awake()
	{
		_searchLocation.OnGeocoderResponse += SearchLocation_OnGeocoderResponse;
	}

	void OnDestroy()
	{
		if (_searchLocation != null)
		{
			_searchLocation.OnGeocoderResponse -= SearchLocation_OnGeocoderResponse;
		}
	}

	void SearchLocation_OnGeocoderResponse(object sender, System.EventArgs e)
	{
		_resultsText.text = JsonConvert.SerializeObject(_searchLocation.Response, Formatting.Indented, JsonConverters.Converters);
	}
}
