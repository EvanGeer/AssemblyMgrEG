using System;
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