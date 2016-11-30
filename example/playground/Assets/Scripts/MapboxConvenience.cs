using UnityEngine;
using Mapbox.Unity;
using Mapbox.Directions;
using Mapbox.Geocoding;

public class MapboxConvenience : MonoBehaviour
{
	[SerializeField]	string _token;

	static FileSource _fileSource;
	public static FileSource FileSource
	{
		get
		{
			return _fileSource;
		}
	}

	static Geocoder _geocoder;
	public static Geocoder Geocoder
	{
		get
		{
			if (_geocoder == null)
			{
				_geocoder = new Geocoder(_fileSource);
			}
			return _geocoder;
		}
	}

	static Directions _directions;
	public static Directions Directions
	{
		get
		{
			if (_directions == null)
			{
				_directions = new Directions(_fileSource);
			}
			return _directions;
		}
	}

	void Awake()
	{
		 //TextWriter.Register();
		_fileSource = new FileSource(this);
		_fileSource.AccessToken = _token;
	}
}
