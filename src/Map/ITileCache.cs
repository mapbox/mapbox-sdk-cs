// https://github.com/BruTile/BruTile

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mapbox.Map
{
	public interface ITileCache<T>
	{
		void Add(CanonicalTileId tileId, T tile);
		void Remove(CanonicalTileId tileId);
		T Get(CanonicalTileId tileId);
	}
}
