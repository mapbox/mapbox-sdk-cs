using UnityEngine;
using Mapbox.Directions;
using System;
using Mapbox;
using UnityEngine.UI;
using Newtonsoft.Json;

public class DirectionsExample : MonoBehaviour{
	[SerializeField]
	Text _resultsText;

	[SerializeField]
	ForwardGeocodeUserInput _startLocationGeocoder;

	[SerializeField]
	ForwardGeocodeUserInput _endLocationGeocoder;

	Directions _directions;

	GeoCoordinate[] _coordinates; 

	DirectionResource _directionResource;

	void Start()
	{
		_directions = MapboxConvenience.Directions;
		_startLocationGeocoder.OnGeocoderResponse += StartLocationGeocoder_OnGeocoderResponse;
		_endLocationGeocoder.OnGeocoderResponse += EndLocationGeocoder_OnGeocoderResponse;

		_coordinates = new GeoCoordinate[2];

		// Can we make routing profiles an enum?
		_directionResource = new DirectionResource(_coordinates, RoutingProfile.Driving);
		_directionResource.Steps = true;
		_directions = MapboxConvenience.Directions;
	}

	void OnDestroy()
	{
		if (_startLocationGeocoder != null)
		{
			_startLocationGeocoder.OnGeocoderResponse -= StartLocationGeocoder_OnGeocoderResponse;
		}

		if (_startLocationGeocoder != null)
		{
			_startLocationGeocoder.OnGeocoderResponse -= EndLocationGeocoder_OnGeocoderResponse;
		}
	}

	void StartLocationGeocoder_OnGeocoderResponse(object sender, EventArgs e)
	{
		_coordinates[0] = _startLocationGeocoder.Coordinate;
		if (ShouldRoute())
		{
			Route();
		}	}

	void EndLocationGeocoder_OnGeocoderResponse(object sender, EventArgs e)
	{
		_coordinates[1] = _endLocationGeocoder.Coordinate;
		if (ShouldRoute())
		{
			Route();
		}
	}

	bool ShouldRoute()
	{
		return _startLocationGeocoder.HasResponse && _endLocationGeocoder.HasResponse;
	}

	void Route()
	{
		_directionResource.Coordinates = _coordinates;
		_directions.Query(_directionResource, HandleDirectionsResponse);
	}
	void HandleDirectionsResponse(DirectionsResponse res)
	{
		var data = JsonConvert.SerializeObject(res, Formatting.Indented, JsonConverters.Converters);
		string sub = data.Substring(0, 5000) + "\n. . . ";
		_resultsText.text = sub;
	}
}
