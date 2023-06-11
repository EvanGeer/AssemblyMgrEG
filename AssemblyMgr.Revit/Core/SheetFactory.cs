using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.Extensions;
using AssemblyMgr.Revit.Data;
using AssemblyMgr.Revit.Extensions;

using Autodesk.Revit.DB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AssemblyMgr.Revit.Core
{
    public class SheetFactory
    {
        private Document _doc;

        public List<AssemblyMgrView> Views { get; }
        public Dictionary<Direction3d, List<AssemblyMgrView>> Schedules { get; }
        public List<AssemblyMgrView> ModelViews { get; }

        public SheetFactory(List<AssemblyMgrView> views)
        {
            _doc = views?.FirstOrDefault(x => x?.View?.Document is Document)?.View.Document;
            Views = views;
            Schedules = views
                .Where(x => x.Definition.Type == ViewPortType.Schedule)
                .GroupBy(x => x.Definition.Direction)
                .ToDictionary(
                    x => x.Key,
                    x => x.Key.IsTowards(Direction3d.N)
                        ? x.OrderByDescending(v => v.Definition.Outline.TopRight.Y).ToList()
                        : x.OrderBy(v => v.Definition.Outline.TopRight.Y).ToList()
                );
            ModelViews = views
                .Where(x => x.Definition.Type != ViewPortType.Schedule)
                .ToList();
        }


        public ViewSheet CreateSheet(AssemblyInstance assembly, ElementId titleBlockId)
        {
            if (assembly == null) return null;

            string name = assembly.Name;
            _doc = assembly.Document;

            var sheet = AssemblyViewUtils.CreateSheet(_doc, assembly.Id, titleBlockId);
            sheet.Name = name;
            sheet.SheetNumber = name;
            //Views.ForEach(x => PlaceView(sheet, x));

            return sheet;
        }

        public void PlaceViews(ViewSheet sheet)
        {
            PlaceSchedules(sheet, Schedules, _doc);
            ModelViews.ForEach(x => PlaceModelViews(sheet, x, _doc));
        }

        public void PlaceSchedules(ViewSheet sheet, Dictionary<Direction3d, List<AssemblyMgrView>> views, Document doc)
        {
            foreach (var direction in views)
            {
                if (!(direction.Value?.Count > 0)) continue;

                var instances = new List<(XYZ dockTo, ScheduleSheetInstance bom)>();

                foreach (var view in direction.Value)
                {
                    if (!(view.View is ViewSchedule schedule && view.Definition is ViewPortSchedule def)) continue;

                    // insert the schedule first, since it doesn't have a size until it's created
                    var bom = ScheduleSheetInstance.Create(doc, sheet.Id, schedule.Id, new XYZ(0, 0, 0));

                    var bbox = bom.get_BoundingBox(sheet);

                    var scheduleOutline = new Box2d((bbox.Min.X, bbox.Min.Y), (bbox.Max.X, bbox.Max.Y));

                    var dockPoint = def.Direction;
                    var unscaledOrigin = def.Outline.GetCoordinate(dockPoint).AsXYZ(0);
                    var desiredOrigin = //= unscaledOrigin * sheet.Outline.Max.U;
                            instances.Count == 0
                            ? new XYZ(unscaledOrigin.X * sheet.Outline.Max.U, unscaledOrigin.Y * sheet.Outline.Max.U, 0)
                            : instances.Last().dockTo;

                    var translation = desiredOrigin - scheduleOutline.GetCoordinate(dockPoint).AsXYZ(0);

                    // Schedules insert point is top-left,
                    // However, we don't care here since we just calculated the translation
                    // to put cornere we want where we want it
                    bom.Point = translation + bom.Point;
                    var nextDock = dockPoint.FlipHorizontal();
                    var nextDockPoint = translation + scheduleOutline.GetCoordinate(nextDock).AsXYZ(0);

                    instances.Add((nextDockPoint, bom));
                }
            }
        }

        private static void PlaceModelViews(ViewSheet sheet, AssemblyMgrView view, Document doc)
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
