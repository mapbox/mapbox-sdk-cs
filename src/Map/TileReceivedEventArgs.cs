using System;

namespace Mapbox.Map
{
	/// <summary>
	/// Event arguments for the <see cref="TileFetcher.TileReceived"/> event
	/// </summary>
	public class TileReceivedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the tile information object
		/// </summary>
		public CanonicalTileId TileId { get; private set; }

		/// <summary>
		/// Gets the actual tile data as a byte Array
		/// </summary>
		public byte[] Tile { get; private set; }

		/// <summary>
		/// Creates an instance of this class
		/// </summary>
		/// <param name="tileInfo">The tile info object</param>
		/// <param name="tile">The tile data</param>
		internal TileReceivedEventArgs(CanonicalTileId tileId, byte[] tile)
		{
			TileId = tileId;
			Tile = tile;
		}
	}
}