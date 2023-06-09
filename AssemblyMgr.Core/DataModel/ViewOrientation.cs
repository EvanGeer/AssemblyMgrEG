using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AssemblyMgr.Core.DataModel
{
    public enum ElevationOrientation
    {
        None = 0,

        // plan view orientations
        Left = Direction3d.W,
        Right = Direction3d.E,
        Front = Direction3d.N,
        Back = Direction3d.S,
    }

    public enum TopBottom
    {
        None = 0,

        Top = 32,
        Bottom = 64,
    }

    public enum ViewCubeCorner
    {
        None = 0,

        TopFrontRight = Direction3d.Up + Direction3d.N + Direction3d.E,
        TopFrontLeft = Direction3d.Up + Direction3d.N + Direction3d.W,
        TopBackRight = Direction3d.Up + Direction3d.S + Direction3d.E,
        TopBackLeft = Direction3d.Up + Direction3d.S + Direction3d.W,

        BottomFrontRight = Direction3d.Down + Direction3d.N + Direction3d.E,
        BottomFrontLeft = Direction3d.Down + Direction3d.N + Direction3d.W,
        BottomBackRight = Direction3d.Down + Direction3d.S + Direction3d.E,
        BottomBackLeft = Direction3d.Down + Direction3d.S + Direction3d.W,
    }

    [Flags]
    public enum Direction3d
    {
        None = 0,

        N = 1,
        S = 2,
        E = 4,
        W = 8,

        Up = 32,

        Down = 64,

        Center = 128,

        //NE = N + E,
        //SE = S + E,
        //NW = N + W,
        //SW = S + W,

        //UpNE = Up + N + E,
        //UpSE = Up + S + E,
        //UpNW = Up + N + W,
        //UpSW = Up + S + W,

        //DownNE = Down + N + E,
        //DownSE = Down + S + E,
        //DownNW = Down + N + W,
        //DownSW = Down + S + W,

        //Front = N,
        //Left = W,
        //Right = E,
        //Back = S,
    }




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
    }

    public enum Box2dLocation
    {
        None = 0,

        TopRight = Direction3d.N | Direction3d.E,
        TopLeft = Direction3d.N | Direction3d.W,
        BottomLeft = Direction3d.S | Direction3d.W,
        BottomRight = Direction3d.S | Direction3d.E,

        Center = Direction3d.Center
    }

    public enum Quadrant
    {
        None = 0,

        TopRight = Direction3d.N | Direction3d.E,
        TopLeft = Direction3d.N | Direction3d.W,
        BottomLeft = Direction3d.S | Direction3d.W,
        BottomRight = Direction3d.S | Direction3d.E,
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

