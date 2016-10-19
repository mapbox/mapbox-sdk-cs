using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Mapbox;
using Mapbox.Geocoding;
using Mapbox.Directions;

public class Playground : MonoBehaviour 
{

	private Directions directions;
	private DirectionResource direction;
	private ReverseGeocodeResource reverseGeocode;
	private ForwardGeocodeResource forwardGeocode;
	private Geocoder geocoder;

	void Start()
    {
		string token = "pk.eyJ1IjoidG1wc2FudG9zIiwiYSI6IkpRS0p1VHcifQ.y5bSLhPlxM21hyiDBizcMg";

		var fileSource = new Mapbox.Unity.FileSource(this);
		fileSource.AccessToken = token;

		string[] types = { "country", "place" };
		string[] country = { "us", "al", "dz" };

		reverseGeocode = new ReverseGeocodeResource(new LatLng(38.897, -77.036));
		
		reverseGeocode.Types = types;

		forwardGeocode = new ForwardGeocodeResource("Minneapolis, MN");

		forwardGeocode.Country = country;

		forwardGeocode.Autocomplete = true;

		geocoder = new Geocoder(fileSource);

		LatLng[] coordinates = { new LatLng(-73.989, 40.733), new LatLng(-74, 40.733) };

		direction = new DirectionResource(coordinates, RoutingProfile.Driving);
		
		direction.Alternatives = true;

		directions = new Directions(fileSource);

	}

	public void Directions()
    {
		print(direction.GetUrl());
		directions.Query(direction,
	        (string json) => {
				var input = this.GetComponent<InputField>();
				input.text = json;
			});
	}

	public void ForwardGeocoder()
	{
		print(forwardGeocode.GetUrl());
		geocoder.Forward(forwardGeocode, (string json) =>
		{
			var input = this.GetComponent<InputField>();
			input.text = json;
		});
	}

	public void ReverseGeocoder()
	{
		print(reverseGeocode.GetUrl());
		geocoder.Reverse(reverseGeocode, (string json) =>
		{
			var input = this.GetComponent<InputField>();
			input.text = json;
		});
	}
}
