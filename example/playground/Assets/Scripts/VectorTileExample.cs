using Mapbox.Map;
using UnityEngine;
using UnityEngine.UI;
using Mapbox;
using System;
using Newtonsoft.Json;

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
		_map.Subscribe(this);
	}

	void SearchLocation_OnGeocoderResponse(object sender, EventArgs e)
	{
		Redraw();
	}

	void Redraw()
	{
		_map.Center = _searchLocation.Coordinate;
	}

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


