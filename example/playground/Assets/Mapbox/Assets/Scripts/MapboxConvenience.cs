using UnityEngine;
using Mapbox.Unity;
using Mapbox.Directions;
using Mapbox.Geocoding;
using System;

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

	/// <summary>
	/// Lazy geocoder.
	/// </summary>
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

	/// <summary>
	/// Lazy Directions.
	/// </summary>
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
		// Forward SDK logs to the Unity Editor.
		//TextWriter.Register();

		if (string.IsNullOrEmpty(_token))
		{
			throw new InvalidTokenException("Please get a token from mapbox.com");		}
		_fileSource = new FileSource(this);
		_fileSource.AccessToken = _token;
	}

	class InvalidTokenException : Exception
	{
		public InvalidTokenException(string message) : base(message)
		{
		}
	}
}
