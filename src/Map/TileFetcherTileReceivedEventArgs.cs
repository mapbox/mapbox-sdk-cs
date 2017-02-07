using System;

namespace Mapbox.Map {
	/// <summary>
	/// Event arguments for the <see cref="TileFetcher.TileReceived"/> event
	/// </summary>
	public class TileFetcherTileReceivedEventArgs : EventArgs {


		/// <summary>
		/// Gets the tile information object
		/// </summary>
		public CanonicalTileId TileId { get; private set; }


		/// <summary>
		/// Gets the actual tile data as a byte Array
		/// </summary>
		public byte[] Tile { get; private set; }


		/// <summary>
		/// Set to true if there was an error downloading the tile
		/// </summary>
		public bool HasError { get { return !string.IsNullOrEmpty(ErrorMessage); } }


		/// <summary>
		/// Error message of tile download failure
		/// </summary>
		public string ErrorMessage { get; private set; }


		/// <summary>
		/// Creates an instance of this class
		/// </summary>
		/// <param name="tileInfo">The tile info object</param>
		/// <param name="tile">The tile data</param>
		internal TileFetcherTileReceivedEventArgs(CanonicalTileId tileId, byte[] tile, string errorMessage = null) {
			TileId = tileId;
			Tile = tile;
			ErrorMessage = errorMessage;
		}


	}
}