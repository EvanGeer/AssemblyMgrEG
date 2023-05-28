using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;

namespace AssemblyMgrShared.DataModel
{
    /// <summary>
    /// Quadrants are laid out similar to a unit circle in trigonometry as follows:
    /// <para /> Q1 = TopRight
    /// <para /> Q2 = TopLeft
    /// <para /> Q3 = BottomLeft
    /// <para /> Q4 = BottomRight
    /// </summary>
    [Flags]
    public enum Quadrant
    {
        None = 0,
        Q1 = 1,
        Q2 = 2,
        Q3 = 4,
        Q4 = 8,
    }

    public enum BoxLocation
    {
        None = 0,
        Center = 1,
        BottomLeft = 2,
        BottomRight = 3,
        TopLeft = 4,
        TopRight = 5,
    }

    //public class Sheet2d
    //{
    //    public Box2d Outline { get; }


    //    public Dictionary<Quadrant, List<Box2d>> ViewPorts { get; private set; }
    //        = new Dictionary<Quadrant, List<Box2d>>()
    //    {

    //    };

    //    public Sheet2d(Box2d outline)
    //    {
    //        Outline = outline;
    //    }

    //    public void AddViewPort(Quadrant quadrant, float height, float width)
    //    {
    //        ViewPorts[quadrant] = Outline.InsertBox(;
    //    }

    //    private Box2d insertBox(Quadrant quadrant, float insertedWidth, float insertedHeight)
    //    {
    //        switch (whereToInsert)
    //        {
    //            case BoxLocation.BottomLeft:
    //                return createBoxBottomLeft(insertedWidth, insertedHeight);
    //            case BoxLocation.BottomRight:
    //                return createBoxBottomRight(insertedWidth, insertedHeight);
    //            case BoxLocation.TopRight:
    //                return createBoxTopRight(insertedWidth, insertedHeight);
    //            case BoxLocation.TopLeft:
    //                return createBoxTopLeft(insertedWidth, insertedHeight);
    //            case BoxLocation.Center:
    //                return createBoxCenter(insertedWidth, insertedHeight);
    //            default:
    //                // this should never happen
    //                Debug.Assert(false, "Invalid location...");
    //                return default;
    //        }
    //    }

    //    private Box2d createBoxCenter(float insertedWidth, float insertedHeight)
    //    {
    //        return new Box2d(
    //            new Vector2(Center.X - (insertedWidth / 2.0f), Center.Y - (insertedHeight / 2.0f)),
    //            new Vector2(Center.X + (insertedWidth / 2.0f), Center.Y + (insertedHeight / 2.0f)));
    //    }

    //    private Box2d createBoxTopLeft(float insertedWidth, float insertedHeight)
    //    {
    //        return new Box2d(
    //            new Vector2(TopLeft.X, TopLeft.Y - insertedHeight),
    //            new Vector2(TopLeft.X + insertedWidth, BottomRight.Y + insertedHeight));
    //    }

    //    private Box2d createBoxTopRight(float insertedWidth, float insertedHeight)
    //    {
    //        return new Box2d(
    //            new Vector2(TopRight.X - insertedWidth, TopRight.Y - insertedHeight),
    //            TopRight);
    //    }

    //    private Box2d createBoxBottomLeft(float insertedWidth, float insertedHeight)
    //    {
    //        return new Box2d(
    //            BottomLeft,
    //            new Vector2(BottomLeft.X + insertedWidth, BottomLeft.Y + insertedHeight));
    //    }

    //    private Box2d createBoxBottomRight(float insertedWidth, float insertedHeight)
    //    {
    //        return new Box2d(
    //            new Vector2(BottomRight.X - insertedWidth, BottomLeft.Y),
    //            new Vector2(BottomRight.X, BottomRight.Y + insertedHeight));
    //    }
    //}
    public class Box2d
    {
        public Box2d(Point pt1, Point pt2)
            : this((pt1.X, pt1.Y),(pt2.X, pt2.Y)) 
        { }
        public Box2d((double X, double Y) pt1, (double X, double Y) pt2)
            : this(new Vector2((float)pt1.X, (float)pt1.Y),
                   new Vector2((float)pt2.X, (float)pt2.Y))
        { }

        public Box2d(Vector2 pt1, Vector2 pt2)
        {
            TopRight =   new Vector2(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y));
            BottomLeft = new Vector2(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y));
        }
        // base properties
        public Vector2 TopRight { get; }
        public Vector2 BottomLeft { get; }
        public float Bottom => BottomLeft.Y;
        public float Left => BottomLeft.X;

        // calculated properties
        public Vector2 Center => (TopRight + BottomLeft) / 2.0f; // midpoint formula 
        public Vector2 TopLeft => new Vector2(BottomLeft.X, TopRight.Y);
        public Vector2 BottomRight => new Vector2(TopRight.X, BottomLeft.Y);
        public float Height => TopRight.Y - BottomLeft.Y;
        public float Width => TopRight.X - BottomLeft.X;

        // this feels like an extension method on the enum...
        public Vector2 GetCoordinate(BoxLocation location)
        {
            switch (location)
            {
                case BoxLocation.BottomLeft: return BottomLeft;
                case BoxLocation.BottomRight: return BottomRight;
                case BoxLocation.TopRight: return TopRight;
                case BoxLocation.TopLeft: return TopLeft;
                case BoxLocation.Center: return Center;
                default:
                    // this should never happen
                    Debug.Assert(false, "Invalid location...");
                    return default;
            }
        }

        public Box2d InsertBox(float insertedWidth, float insertedHeight, BoxLocation whereToInsert)
        {
            switch (whereToInsert)
            {
                case BoxLocation.BottomLeft:
                    return createBoxBottomLeft(insertedWidth, insertedHeight);
                case BoxLocation.BottomRight:
                    return createBoxBottomRight(insertedWidth, insertedHeight);
                case BoxLocation.TopRight:
                    return createBoxTopRight(insertedWidth, insertedHeight);
                case BoxLocation.TopLeft:
                    return createBoxTopLeft(insertedWidth, insertedHeight);
                case BoxLocation.Center:
                    return createBoxCenter(insertedWidth, insertedHeight);
                default:
                    // this should never happen
                    Debug.Assert(false, "Invalid location...");
                    return default;
            }
        }

        private Box2d createBoxCenter(float insertedWidth, float insertedHeight)
        {
            return new Box2d(
                new Vector2(Center.X - (insertedWidth / 2.0f), Center.Y - (insertedHeight / 2.0f)),
                new Vector2(Center.X + (insertedWidth / 2.0f), Center.Y + (insertedHeight / 2.0f)));
        }

        private Box2d createBoxTopLeft(float insertedWidth, float insertedHeight)
        {
            return new Box2d(
                new Vector2(TopLeft.X, TopLeft.Y - insertedHeight),
                new Vector2(TopLeft.X + insertedWidth, BottomRight.Y + insertedHeight));
        }

        private Box2d createBoxTopRight(float insertedWidth, float insertedHeight)
        {
            return new Box2d(
                new Vector2(TopRight.X - insertedWidth, TopRight.Y - insertedHeight),
                TopRight);
        }

        private Box2d createBoxBottomLeft(float insertedWidth, float insertedHeight)
        {
            return new Box2d(
                BottomLeft,
                new Vector2(BottomLeft.X + insertedWidth, BottomLeft.Y + insertedHeight));
        }

        private Box2d createBoxBottomRight(float insertedWidth, float insertedHeight)
        {
            return new Box2d(
                new Vector2(BottomRight.X - insertedWidth, BottomLeft.Y),
                new Vector2(BottomRight.X, BottomRight.Y + insertedHeight));
        }
    }



    public static class SpoolSheetDefinitionExtensions
    {
        const double Spacing = 0.01;


        public static (double X, double Y, double Z) GetBOMInsertionPoint(this ISpoolSheetDefinition definition, double sheetMaxU, double sheetMaxV)
        {
            //var bomWidth = definition.GetBOMWidth();
            var X = sheetMaxU; // - bomWidth - Spacing;
            var Y = sheetMaxV - Spacing;
            var Z = 0;

            return (X, Y, Z);
        }

        public static double GetBOMWidth(this ISpoolSheetDefinition definition)
        {
            var bomWidth = definition.BOMFields.Sum(x => x.ColumnWidth);
            return bomWidth;

        }
    }
}