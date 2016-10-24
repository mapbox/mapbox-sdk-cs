using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Mapbox;
using Mapbox.Geocoding;
using Mapbox.Directions;
using Mapbox.Map;

public class Playground : MonoBehaviour, Mapbox.IObserver<VectorTile>
{

    private Directions directions;
    private DirectionResource direction;
    private ReverseGeocodeResource reverseGeocode;
    private ForwardGeocodeResource forwardGeocode;
    private Geocoder geocoder;
    private Map<VectorTile> map;

    void Start()
    {
        // Token created only for these examples, will get rotated. Do not use in production.
        string token = "pk.eyJ1IjoidG1wc2FudG9zIiwiYSI6ImNpdW56YmxrYTAwMTUydGw4bjdvbDB0djkifQ.sSt9IrAODfFnkzMsPHRU1A";

        var fileSource = new Mapbox.Unity.FileSource(this);
        fileSource.AccessToken = token;

        string[] types = { "country", "place" };
        string[] country = { "us", "al", "dz" };

        reverseGeocode = new ReverseGeocodeResource(new GeoCoordinate(38.897, -77.036));
        reverseGeocode.Types = types;

        forwardGeocode = new ForwardGeocodeResource("Minneapolis, MN");
        forwardGeocode.Country = country;
        forwardGeocode.Autocomplete = true;

        geocoder = new Geocoder(fileSource);

        GeoCoordinate[] coordinates = { new GeoCoordinate(-73.989, 40.733), new GeoCoordinate(-74, 40.733) };

        direction = new DirectionResource(coordinates, RoutingProfile.Driving);
        direction.Alternatives = true;

        directions = new Directions(fileSource);

        map = new Map<VectorTile>(fileSource);
        map.Subscribe(this);
    }

    void SetInputField(string text)
    {
        var input = this.GetComponent<InputField>();
        input.text = text;
    }

    void AppendToInputField(string text)
    {
        var input = this.GetComponent<InputField>();
        input.text += "\n" + text;
    }

    public void Directions()
    {
        print(direction.GetUrl());
        directions.Query(direction, SetInputField);
    }

    public void ForwardGeocoder()
    {
        print(forwardGeocode.GetUrl());
        geocoder.Forward(forwardGeocode, SetInputField);
    }

    public void ReverseGeocoder()
    {
        print(reverseGeocode.GetUrl());
        geocoder.Reverse(reverseGeocode, SetInputField);
    }

    public void FetchTiles()
    {
        SetInputField(""); // clear
        map.Zoom = 0;

        map.GeoCoordinateBounds = GeoCoordinateBounds.World();
        map.Zoom = 3;
    }

    public void OnCompleted()
    {
        AppendToInputField("Completed.");
    }

    public void OnNext(VectorTile tile)
    {
        var text = tile.ToString() + ": ";

        if (tile.Error != null)
        {
            text += tile.Error;
        }
        else
        {
            text += tile.Data.Length + " bytes";
        }

        AppendToInputField(text);
    }

    public void OnError(string error)
    {
        AppendToInputField("Error.");
    }
}
