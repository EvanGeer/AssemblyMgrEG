using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Geometry;

namespace AssemblyMgr.Core.Extensions
{
    public static class Direction3dExtensions
    {
        public static IEnumerable<T> GetFlags<T>(this T mask)
            where T : Enum
        {
            return Enum.GetValues(typeof(T))
                                 .Cast<Enum>()
                                 .Where(m => mask.HasFlag(m))
                                 .Cast<T>();
        }

        private static Vector3 asVector(this Direction3d direction)
        {
            switch (direction)
            {
                case Direction3d.N: return Vector3.UnitY;
                case Direction3d.S: return -1 * Vector3.UnitY;

                case Direction3d.E: return Vector3.UnitX;
                case Direction3d.W: return -1 * Vector3.UnitX;

                case Direction3d.Up: return Vector3.UnitZ;
                case Direction3d.Down: return -1 * Vector3.UnitZ;
                default: return Vector3.Zero;
            }
        }

        public static Vector3 AsVector(this Direction3d direction)
        {
            var flags = direction.GetFlags();
            var directions = flags.Select(x => x.asVector()).ToList();
            var summedDirection = directions.Aggregate((total, current) => total + current);
            return summedDirection;
        }

        public static Direction3d FlipHorizontal(this Direction3d direction)
        {
            var flags = direction.GetFlags();
            var flippedFlags = flags.Select(x => 
                x == Direction3d.N ? Direction3d.S
                : x == Direction3d.S ? Direction3d.N
                : x).ToList();

            var flipped = flippedFlags.Aggregate((total, current) => total | current);
            return flipped;
        }

        public static bool IsTowards(this Direction3d direction, Direction3d towards)
        {
            var flags = direction.GetFlags();
            return flags.Contains(towards);
        }
    }
}




//case Direction3d.NE:
//    return Direction3d.N.AsVector()
//         + Direction3d.E.AsVector();

//case Direction3d.SE:
//    return Direction3d.S.AsVector()
//         + Direction3d.E.AsVector();

//case Direction3d.NW:
//    return Direction3d.N.AsVector()
//         + Direction3d.W.AsVector();

//case Direction3d.SW:
//    return Direction3d.S.AsVector()
//         + Direction3d.W.AsVector();


//case Direction3d.UpSW:
//    return Direction3d.S.AsVector()
//         + Direction3d.W.AsVector()
//         + Direction3d.Up.AsVector();

//case Direction3d.UpNE:
//    return Direction3d.N.AsVector()
//         + Direction3d.E.AsVector()
//         + Direction3d.Up.AsVector();

//case Direction3d.UpSE:
//    return Direction3d.S.AsVector()
//         + Direction3d.E.AsVector()
//         + Direction3d.Up.AsVector();

//case Direction3d.UpNW:
//    return Direction3d.N.AsVector()
//         + Direction3d.W.AsVector()
//         + Direction3d.Up.AsVector();


//case Direction3d.DownSW:
//    return Direction3d.S.AsVector()
//         + Direction3d.W.AsVector()
//         + Direction3d.Down.AsVector();

//case Direction3d.DownNE:
//    return Direction3d.N.AsVector()
//         + Direction3d.E.AsVector()
//         + Direction3d.Down.AsVector();

//case Direction3d.DownSE:
//    return Direction3d.S.AsVector()
//         + Direction3d.E.AsVector()
//         + Direction3d.Down.AsVector();

//case Direction3d.DownNW:
//    return Direction3d.N.AsVector()
//         + Direction3d.W.AsVector()
//         + Direction3d.Down.AsVector();

