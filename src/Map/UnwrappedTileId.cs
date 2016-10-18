//-----------------------------------------------------------------------
// <copyright file="UnwrappedTileId.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    internal struct UnwrappedTileId
    {
        public readonly int Z;
        public readonly int X;
        public readonly int Y;

        public UnwrappedTileId(int z, int x, int y)
        {
            this.Z = z;
            this.X = x;
            this.Y = y;
        }

        public CanonicalTileId Canonical
        {
            get
            {
                return new CanonicalTileId(this);
            }
        }

        public override string ToString()
        {
            return this.Z + "/" + this.X + "/" + this.Y;
        }
    }
}
