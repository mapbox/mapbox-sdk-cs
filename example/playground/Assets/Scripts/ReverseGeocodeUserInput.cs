using UnityEngine;
using UnityEngine.UI;
using Mapbox.Geocoding;
using System;
using Mapbox;

[RequireComponent(typeof(InputField))]
public class ReverseGeocodeUserInput : MonoBehaviour
{
	InputField _inputField;

	ReverseGeocodeResource _resource;

	GeoCoordinate _coordinate;

	bool _hasResponse;
	public bool HasResponse
	{
		get
		{
			return _hasResponse;
		}
	}

	public ReverseGeocodeResponse Response { get; private set;}

	public event EventHandler<EventArgs> OnGeocoderResponse;

	void Awake()
	{
		_inputField = GetComponent<InputField>();
		_inputField.onEndEdit.AddListener(HandleUserInput);
		_resource = new ReverseGeocodeResource(_coordinate);
	}

	void HandleUserInput(string searchString)
	{
		_hasResponse = false;
		if (!string.IsNullOrEmpty(searchString))
		{
			var latLon = searchString.Split(',');
			_coordinate.Latitude = double.Parse(latLon[0]);
			_coordinate.Longitude = double.Parse(latLon[1]);
			_resource.Query = _coordinate;
			MapboxConvenience.Geocoder.Geocode(_resource, HandleGeocoderResponse);
		}
	}

	void HandleGeocoderResponse(ReverseGeocodeResponse res)
	{
		_hasResponse = true;
		Response = res;
		if (OnGeocoderResponse != null)
		{
			OnGeocoderResponse(this, EventArgs.Empty);		}
	}
}