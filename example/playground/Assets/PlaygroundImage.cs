using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Mapbox;
using Mapbox.Map;

public class PlaygroundImage : MonoBehaviour, Mapbox.IObserver<RasterTile>
{

    private Map<RasterTile> map;

    void Start()
    {
        // Token created only for these examples, will get rotated. Do not use in production.
        string token = "pk.eyJ1IjoidG1wc2FudG9zIiwiYSI6ImNpdW56YmxrYTAwMTUydGw4bjdvbDB0djkifQ.sSt9IrAODfFnkzMsPHRU1A";

        var fileSource = new Mapbox.Unity.FileSource(this);
        fileSource.AccessToken = token;

        map = new Map<RasterTile>(fileSource);
        map.Subscribe(this);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 100);
    }

    public void GoToSanFrancisco()
    {
        map.Center = new GeoCoordinate(37.7833, -122.4167);
        map.Zoom = 11;
    }

    public void GoToHelsinki()
    {
        map.Center = new GeoCoordinate(60.163200, 24.937700);
        map.Zoom = 13;
    }

    public void GoToDC()
    {
        map.Center = new GeoCoordinate(38.889428,-77.009961);
        map.Zoom = 16;
    }

    public void ChangeSource()
    {
        // Setting to null == default source (satellite).
        map.Source = map.Source == null ? "mapbox.terrain-rgb" : null;
    }

    public void OnNext(RasterTile tile)
    {
        if (tile.CurrentState != Tile.State.Loaded || tile.Error != null)
        {
            return;
        }

        Texture2D texture = new Texture2D(256, 256);
        texture.LoadImage(tile.Data);

        GetComponent<Image>().sprite =
            Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
    }
}
