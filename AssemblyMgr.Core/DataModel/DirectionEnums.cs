using System;
using System.Text;

namespace AssemblyMgr.Core.DataModel
{
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

    public enum ElevationOrientation
    {
        None = 0,

        // plan view orientations
        Left = Direction3d.W,
        Right = Direction3d.E,
        Front = Direction3d.N,
        Back = Direction3d.S,
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






