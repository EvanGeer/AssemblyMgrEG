using System;
using System.Numerics;


namespace AssemblyMgr.Core.Geometry
{
    public class Box2d
    {
        // base properties

        public float Max_X => TopRight.X;
        public float Max_Y => TopRight.Y;
        public float Min_X => BottomLeft.X;
        public float Min_Y => BottomLeft.Y;
        public Vector2 TopRight { get; set; }
        public Vector2 BottomLeft { get; set; }


        // calculated properties
        public float Bottom => BottomLeft.Y;
        public float Left => BottomLeft.X;
        public Vector2 Center => (TopRight + BottomLeft) / 2.0f; // midpoint formula 
        public Vector2 TopLeft => new Vector2(BottomLeft.X, TopRight.Y);
        public Vector2 BottomRight => new Vector2(TopRight.X, BottomLeft.Y);
        public float Height => TopRight.Y - BottomLeft.Y;
        public float Width => TopRight.X - BottomLeft.X;

        public Box2d((double X, double Y) pt1, (double X, double Y) pt2)
            : this(new Vector2((float)pt1.X, (float)pt1.Y),
                   new Vector2((float)pt2.X, (float)pt2.Y))
        { }

        public Box2d(Vector2 pt1, Vector2 pt2)
        {
            TopRight = new Vector2(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y));
            BottomLeft = new Vector2(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y));
        }

        public Box2d() { }

        //public Box2d InsertBox(float insertedWidth, float insertedHeight, Box2dLocation whereToInsert)
        //{
        //    switch (whereToInsert)
        //    {
        //        case Box2dLocation.BottomLeft:
        //            return createBoxBottomLeft(insertedWidth, insertedHeight);
        //        case Box2dLocation.BottomRight:
        //            return createBoxBottomRight(insertedWidth, insertedHeight);
        //        case Box2dLocation.TopRight:
        //            return createBoxTopRight(insertedWidth, insertedHeight);
        //        case Box2dLocation.TopLeft:
        //            return createBoxTopLeft(insertedWidth, insertedHeight);
        //        case Box2dLocation.Center:
        //            return createBoxCenter(insertedWidth, insertedHeight);
        //        default:
        //            // this should never happen
        //            Debug.Assert(false, "Invalid location...");
        //            return default;
        //    }
        //}

        //public Box2d CreateBox(float insertedWidth, float insertedHeight, Quadrant whereToInsert)
        //{
        //    switch (whereToInsert)
        //    {
        //        case Quadrant.BottomLeft:
        //            return createBoxBottomLeft(insertedWidth, insertedHeight);
        //        case Quadrant.BottomRight:
        //            return createBoxBottomRight(insertedWidth, insertedHeight);
        //        case Quadrant.TopRight:
        //            return createBoxTopRight(insertedWidth, insertedHeight);
        //        case Quadrant.TopLeft:
        //            return createBoxTopLeft(insertedWidth, insertedHeight);
        //        default:
        //            // this should never happen
        //            Debug.Assert(false, "Invalid location...");
        //            return default;
        //    }
        //}

        //private Box2d createBoxCenter(float insertedWidth, float insertedHeight)
        //{
        //    return new Box2d(
        //        new Vector2(Center.X - insertedWidth / 2.0f, Center.Y - insertedHeight / 2.0f),
        //        new Vector2(Center.X + insertedWidth / 2.0f, Center.Y + insertedHeight / 2.0f));
        //}

        //private Box2d createBoxTopLeft(float insertedWidth, float insertedHeight)
        //{
        //    return new Box2d(
        //        new Vector2(TopLeft.X, TopLeft.Y - insertedHeight),
        //        new Vector2(TopLeft.X + insertedWidth, BottomRight.Y + insertedHeight));
        //}

        //private Box2d createBoxTopRight(float insertedWidth, float insertedHeight)
        //{
        //    return new Box2d(
        //        new Vector2(TopRight.X - insertedWidth, TopRight.Y - insertedHeight),
        //        TopRight);
        //}

        //private Box2d createBoxBottomLeft(float insertedWidth, float insertedHeight)
        //{
        //    return new Box2d(
        //        BottomLeft,
        //        new Vector2(BottomLeft.X + insertedWidth, BottomLeft.Y + insertedHeight));
        //}

        //private Box2d createBoxBottomRight(float insertedWidth, float insertedHeight)
        //{
        //    return new Box2d(
        //        new Vector2(BottomRight.X - insertedWidth, BottomLeft.Y),
        //        new Vector2(BottomRight.X, BottomRight.Y + insertedHeight));
        //}
        /// <summary>Scales the box in the specified amount
        /// <para/>i.e. divides the value out of each corner</summary>
        public static Box2d operator /(Box2d box, double value)
            => box is null ? null : new Box2d(box.TopLeft / (float)value, box.BottomRight / (float)value);
        /// <summary>Scales the box in the specified amount
        /// <para/>i.e. multiplies the vector to each corner</summary>
        public static Box2d operator *(Box2d box, double value) 
            => box is null ? null : new Box2d(box.TopLeft * (float)value, box.BottomRight * (float)value);
        /// <summary>Moves the box in the specified direction
        /// <para/>i.e. adds the vector to each corner</summary>
        public static Box2d operator +(Box2d box, Vector2 translation)
            => box is null ? null : new Box2d(box.BottomLeft + translation, box.TopRight + translation);
    }
}