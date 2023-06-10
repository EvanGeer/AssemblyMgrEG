using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.Extensions;
using AssemblyMgr.Revit.Data;
using AssemblyMgr.Revit.Extensions;

using Autodesk.Revit.DB;

using System;
using System.Collections.Generic;

namespace AssemblyMgr.Revit.Core
{
    public class SheetFactory
    {
        public List<AssemblyMgrView> Views { get; }
        public SheetFactory(List<AssemblyMgrView> views)
        {
            Views = views;
        }


        public ViewSheet Create(AssemblyInstance assembly, ElementId titleBlockId)
        {
            if (assembly == null) return null;

            string name = assembly.Name;
            var doc = assembly.Document;

            var sheet = AssemblyViewUtils.CreateSheet(doc, assembly.Id, titleBlockId);
            sheet.Name = name;
            sheet.SheetNumber = name;
            Views.ForEach(x => PlaceView(sheet, x));

            return sheet;
        }

        public void PlaceView(ViewSheet sheet, AssemblyMgrView view)
        {
            var doc = sheet.Document;

            if (view.View is ViewSchedule schedule && view.Definition is ViewPortSchedule def)
            {
                // insert the schedule first, since it doesn't have a size until it's created
                var bom = ScheduleSheetInstance.Create(doc, sheet.Id, schedule.Id, new XYZ(0, 0, 0));

                var bbox = bom.get_BoundingBox(sheet);
                var scheduleOutline = new Box2d((bbox.Min.X, bbox.Min.Y), (bbox.Max.X, bbox.Max.Y));

                var unscaledOrigin = view.Definition.Outline.GetCoordinate(def.DockPoint).AsXYZ(0);
                var origin = unscaledOrigin * sheet.Outline.Max.U;
                           //= new XYZ(unscaledOrigin.X * sheet.Outline.Max.U, unscaledOrigin.Y * sheet.Outline.Max.U, 0);

                var translation = origin - scheduleOutline.GetCoordinate(def.DockPoint).AsXYZ(0);

                // Schedules insert point is top-left,
                // However, we don't care here since we just calculated the translation
                // to put cornere we want where we want it
                bom.Point = bom.Point + translation;
            }
            else
            {
                var sheetOutline = sheet.Outline.AsBox2d();
                var viewOutline = view.View.Outline.AsBox2d();

                var unscaledOrigin = view.Definition.Outline.Center.AsXYZ(z: 0);
                var origin = new XYZ(unscaledOrigin.X * sheet.Outline.Max.U, unscaledOrigin.Y * sheet.Outline.Max.U, 0);

                var desiredWidth = view.Definition.Outline.Width * sheetOutline.Width;
                var actualWidth = viewOutline.Width * view.View.Scale;

                var desiredHeight = view.Definition.Outline.Height * sheetOutline.Height;
                var actualHeight = viewOutline.Height * view.View.Scale;

                var scale = Math.Max(actualWidth / desiredWidth, actualHeight / desiredHeight);
                view.View.Scale = (int)scale;

                // View insert point is the center
                var viewport = Viewport.Create(doc, sheet.Id, view.View.Id, origin);
            }
        }
    }
}
