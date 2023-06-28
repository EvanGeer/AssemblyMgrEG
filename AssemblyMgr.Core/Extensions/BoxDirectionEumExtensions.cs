using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Geometry;
using System.Diagnostics;
using System.Numerics;

namespace AssemblyMgr.Core.Extensions
{
    public static class BoxDirectionEumExtensions
    {


        public static Vector2 GetCoordinate(this Quadrant location, Box2d box)
            => box.GetCoordinate((Box2dLocation)location);
        public static Vector2 GetCoordinate(this Box2dLocation location, Box2d box)
            => box.GetCoordinate(location);
        public static Vector2 GetCoordinate(this Box2d box, Quadrant location)
            => box.GetCoordinate((Box2dLocation)location);
        public static Vector2 GetCoordinate(this Box2d box, Direction3d location)
            => box.GetCoordinate((Box2dLocation)location);
        public static Vector2 GetCoordinate(this Box2d box, Box2dLocation location)
        {
            switch (location)
            {
                case Box2dLocation.BottomLeft: return box.BottomLeft;
                case Box2dLocation.BottomRight: return box.BottomRight;
                case Box2dLocation.TopRight: return box.TopRight;
                case Box2dLocation.TopLeft: return box.TopLeft;
                case Box2dLocation.Center: return box.Center;
                default:
                    // this should never happen
                    Debug.Assert(false, "Invalid location...");
                    return default;
            }
        }

    }
}
