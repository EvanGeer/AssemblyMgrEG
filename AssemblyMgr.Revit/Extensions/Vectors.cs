using System.Numerics;
using AssemblyMgr.Core.Geometry;
using Autodesk.Revit.DB;

namespace AssemblyMgr.Revit.Extensions
{
    public static class Vectors
    {

        public static Vector2 AsVector2(this UV revitVector)
        {
            return new Vector2((float)revitVector.U, (float)revitVector.V);
        }


        public static UV AsUV(this Vector2 numericsVector)
        {
            return new UV(numericsVector.X, numericsVector.Y);
        }

        public static Box2d AsBox2d(this BoundingBoxUV outline)
        {
            return new Box2d(outline.Min.AsVector2(), outline.Max.AsVector2());
        }

        public static BoundingBoxUV AsBoundingBoxUV(this Box2d box)
        {
            return new BoundingBoxUV(box.BottomLeft.X, box.BottomLeft.Y, box.TopRight.X, box.TopRight.Y);
        }
    }
}
