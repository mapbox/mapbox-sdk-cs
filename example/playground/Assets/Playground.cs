using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Mapbox;

public class Playground : MonoBehaviour 
{
	private Directions directions;
	private Geocoder geocoder;

	void Start()
    {
		string token = "pk.eyJ1IjoidG1wc2FudG9zIiwiYSI6IkpRS0p1VHcifQ.y5bSLhPlxM21hyiDBizcMg";

		var fileSource = new Mapbox.Unity.FileSource(this);
		fileSource.SetAccessToken(token);

		directions = new Directions(fileSource);
		geocoder = new Geocoder(fileSource);
	}

	public void Directions()
    {
		directions.Query(new LatLng (38.897, -77.036), new LatLng(38.963, -77.024), RoutingProfile.Driving,
	        (string json) => {
				var input = this.GetComponent<InputField>();
				input.text = json;
			});
	}

	public void ReverseGeocoder()
	{
		geocoder.Reverse(new LatLng(38.897, -77.036), (string json) =>
		{
			var input = this.GetComponent<InputField>();
			input.text = json;
		});
	}
}
