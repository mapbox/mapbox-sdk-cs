using Mapbox.Map;
using UnityEngine;
using UnityEngine.UI;
using Mapbox;
using System;

public class RasterTileExample : MonoBehaviour, IObserver<RasterTile>
{
	[SerializeField]	ForwardGeocodeUserInput _searchLocation;

	[SerializeField]
	Slider _zoomSlider;

	[SerializeField]
	Toggle _terrainToggle;

	[SerializeField]
	RawImage _imageContainer;

	Map<RasterTile> _map;

	void Awake()
	{
		_searchLocation.OnGeocoderResponse += SearchLocation_OnGeocoderResponse;
		_terrainToggle.onValueChanged.AddListener(ToggleTerrain);
		_zoomSlider.onValueChanged.AddListener(AdjustZoom);
	}
	void OnDestroy()
	{
		if (_searchLocation != null)
		{
			_searchLocation.OnGeocoderResponse -= SearchLocation_OnGeocoderResponse;
		}
	}

	void Start()
	{
		_map = new Map<RasterTile>(MapboxConvenience.FileSource);
		_map.Subscribe(this);
	}

	void SearchLocation_OnGeocoderResponse(object sender, EventArgs e)
	{
		Redraw();
	}

	void AdjustZoom(float value)
	{
		Redraw();
	}

	void Redraw()
	{
		_map.Center = _searchLocation.Coordinate;
		_map.Zoom = (int)_zoomSlider.value;
	}

	void ToggleTerrain(bool value)
	{
		// Setting to null == default source (satellite).
		_map.Source = _map.Source == null ? "mapbox.terrain-rgb" : null;
	}

	public void OnNext(RasterTile tile)
	{
		if (tile.CurrentState != Tile.State.Loaded || tile.Error != null)
		{
			return;
		}

		// Can we utility this? Should users have to know source size?
		var texture = new Texture2D(256, 256);
		texture.LoadImage(tile.Data);
		_imageContainer.texture = texture;
	}
}


