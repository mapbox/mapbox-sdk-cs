using Mapbox.Map;
using UnityEngine;
using UnityEngine.UI;
using Mapbox;
using System;
using Mapbox.Json;

public class VectorTileExample : MonoBehaviour, IObserver<VectorTile>
{
	[SerializeField]	ForwardGeocodeUserInput _searchLocation;

	[SerializeField]
	Text _resultsText;

	Map<VectorTile> _map;

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

	void Start()
	{
		_map = new Map<VectorTile>(MapboxConvenience.FileSource);

		// This marks us an an observer to map.
		// We will get each tile in OnNext(VectorTile tile) as they become available.
		_map.Subscribe(this);
	}

	/// <summary>
	/// Search location was changed.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	void SearchLocation_OnGeocoderResponse(object sender, EventArgs e)
	{
		Redraw();
	}

	/// <summary>
	/// Request _map to update its tile data with new coordinates.
	/// </summary>
	void Redraw()
	{
		_map.Center = _searchLocation.Coordinate;
	}


	/// <summary>
	/// Handle tile data from _map as they become available.
	/// </summary>
	/// <param name="tile">Tile.</param>
	public void OnNext(VectorTile tile)
	{
		if (tile.CurrentState != Tile.State.Loaded || tile.Error != null)
		{
			return;
		}

		var data = JsonConvert.SerializeObject(tile.Data, Formatting.Indented, JsonConverters.Converters);
		string sub = data.Substring(0, 5000) + "\n. . . ";
		_resultsText.text = sub;
	}
}


