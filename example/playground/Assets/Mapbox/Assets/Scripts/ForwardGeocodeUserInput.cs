using UnityEngine;
using UnityEngine.UI;
using Mapbox.Geocoding;
using System;
using Mapbox;

/// <summary>
/// Peforms a forward geocoder request (search by name) whenever the InputField on *this*
/// gameObject is finished with an edit.
/// </summary>
[RequireComponent(typeof(InputField))]
public class ForwardGeocodeUserInput : MonoBehaviour
{
	InputField _inputField;

	ForwardGeocodeResource _resource;

	Geocoder _geocoder;

	GeoCoordinate _coordinate;
	public GeoCoordinate Coordinate
	{
		get
		{
			return _coordinate;
		}
	}

	bool _hasResponse;
	public bool HasResponse
	{
		get
		{
			return _hasResponse;
		}
	}

	public ForwardGeocodeResponse Response { get; private set;}

	public event EventHandler<EventArgs> OnGeocoderResponse;

	void Awake()
	{
		_inputField = GetComponent<InputField>();
		_inputField.onEndEdit.AddListener(HandleUserInput);
		_resource = new ForwardGeocodeResource("");
	}

	void Start()
	{
		_geocoder = MapboxConvenience.Geocoder;
	}

	/// <summary>
	/// An edit was made to the InputField.
	/// Unity will send the string from _inputField.
	/// Make geocoder query.
	/// </summary>
	/// <param name="searchString">Search string.</param>
	void HandleUserInput(string searchString)
	{
		_hasResponse = false;
		if (!string.IsNullOrEmpty(searchString))
		{
			_resource.Query = searchString;
			_geocoder.Geocode(_resource, HandleGeocoderResponse);
		}
	}

	/// <summary>
	/// Handles the geocoder response by updating coordinates and notifying observers.
	/// </summary>
	/// <param name="res">Res.</param>
	void HandleGeocoderResponse(ForwardGeocodeResponse res)
	{
		_hasResponse = true;
		_coordinate = res.Features[0].Center;
		Response = res;
		if (OnGeocoderResponse != null)
		{
			OnGeocoderResponse(this, EventArgs.Empty);		}
	}
}