//https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/TileReceivedEventArgs.cs

using System;

namespace Mapbox.Map
{
	/// <summary>
	/// Event arguments for the <see cref="TileFetcher.TileReceived"/> event
	/// </summary>
	public class MapTileReceivedEventArgs<T> : EventArgs
	{

		public MapTileReceivedEventArgs(T tile)
		{
			Tile = tile;
		}
		/// <summary>
		/// Gets the actual tile data as a byte Array
		/// </summary>
		public T Tile { get; private set; }

	}
}