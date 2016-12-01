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

	/// <summary>
	/// New search location has become available, begin a new _map query.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	void SearchLocation_OnGeocoderResponse(object sender, EventArgs e)
	{
		Redraw();
	}

	/// <summary>
	/// Zoom was modified by the slider, begin a new _map query.
	/// </summary>
	/// <param name="value">Value.</param>
	void AdjustZoom(float value)
	{
		Redraw();
	}

	void Redraw()
	{
		_map.Center = _searchLocation.Coordinate;
		_map.Zoom = (int)_zoomSlider.value;
	}

	/// <summary>
	/// Terrain toggle was modified, begin a new _map query.
	/// </summary>
	/// <param name="value">If set to <c>true</c> value.</param>
	void ToggleTerrain(bool value)
	{
		// Setting to null == default source (satellite).
		_map.Source = _map.Source == null ? "mapbox.terrain-rgb" : null;
	}

	/// <summary>
	/// Update the texture with new data.
	/// </summary>
	/// <param name="tile">Tile.</param>
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


