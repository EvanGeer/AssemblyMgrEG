using System.Numerics;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Geometry;
using Autodesk.Revit.DB;

namespace AssemblyMgr.Revit.Extensions
{
    public static class RevitApiConverters
    {

        public static Vector2 AsVector2(this UV revitVector)
        {
            return new Vector2((float)revitVector.U, (float)revitVector.V);
        }
        public static Vector3 AsVector3(this XYZ revitVector)
        {
            return new Vector3((float)revitVector.X, (float)revitVector.Y, (float)revitVector.Z);
        }

        public static Box3d AsBox3d(this BoundingBoxXYZ boxXYZ)
        {
            var max = boxXYZ.Max.AsVector3();
            var min = boxXYZ.Min.AsVector3();

            return new Box3d(max, min);
        }

        public static XYZ GetCenter(this BoundingBoxXYZ boxXYZ)
            => boxXYZ.AsBox3d().Center.AsXYZ();

        public static XYZ AsXYZ(this Vector3 revitVector)
        {
            return new XYZ(revitVector.X, revitVector.Y, revitVector.Z);
        }

        public static UV AsUV(this Vector2 numericsVector)
        {
            return new UV(numericsVector.X, numericsVector.Y);
        }
        public static XYZ AsXYZ(this Vector2 numericsVector, double z)
        {
            return new XYZ(numericsVector.X, numericsVector.Y, z);
        }

        public static Box2d AsBox2d(this BoundingBoxUV outline)
        {
            return new Box2d(outline.Min.AsVector2(), outline.Max.AsVector2());
        }

        public static BoundingBoxUV AsBoundingBoxUV(this Box2d box)
        {
            return new BoundingBoxUV(box.BottomLeft.X, box.BottomLeft.Y, box.TopRight.X, box.TopRight.Y);
        }

        public static bool TryAsAssemblyDetailViewOrientation(this ElevationOrientation orientation, out AssemblyDetailViewOrientation assemblyDetailViewOrientation)
        {
            switch (orientation)
            {
                case ElevationOrientation.Left: 
                    assemblyDetailViewOrientation = AssemblyDetailViewOrientation.ElevationLeft;
                    return true;
                case ElevationOrientation.Right:
                    assemblyDetailViewOrientation = AssemblyDetailViewOrientation.ElevationRight;
                    return true;
                case ElevationOrientation.Front: 
                    assemblyDetailViewOrientation = AssemblyDetailViewOrientation.ElevationFront;
                    return true;
                case ElevationOrientation.Back:
                    assemblyDetailViewOrientation = AssemblyDetailViewOrientation.ElevationBack;
                    return true;
                default:
                    assemblyDetailViewOrientation = default;
                    return false;
            }
        }

    }
}
