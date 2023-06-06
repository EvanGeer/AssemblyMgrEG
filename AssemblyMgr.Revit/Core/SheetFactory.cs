using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Revit.Data;
using AssemblyMgr.Revit.Extensions;
using AssemblyMgr.UI.ViewModels;
using Autodesk.Revit.DB;

namespace AssemblyMgr.Revit.Core
{
    public class SheetFactory
    {
        //public ViewSheet Sheet { get; private set; }
        //public AssemblyInstance Assembly { get; }
        public List<AssemblyMgrView> Views { get; }
        public SheetFactory(List<AssemblyMgrView> views)
        {
            //CreateSheet();
            //Assembly = assembly;
            Views = views;
        }


        public ViewSheet Create(AssemblyInstance assembly, ElementId titleBlockId)
        {
            // To-Do: Clean up layout stuff, probably could parameterize all this and put in form and/or add some intelligence
            // To-Do: Break this out into the business logic module and cover the functionality with unit tests
            if (assembly == null) return null;

            string name = assembly.Name;
            var doc = assembly.Document;

            //using (var t = new Transaction(doc, $"Build Sheet: {name}"))
            //{
            //    t.Start();
            var sheet = AssemblyViewUtils.CreateSheet(doc, assembly.Id, titleBlockId);
            sheet.Name = name;
            sheet.SheetNumber = name;
            Views.ForEach(x => PlaceView(sheet, x));

            //t.Commit();
            return sheet;
            //}

            //using (Transaction t = new Transaction(doc, "Assembly Manager: ExportImage Sheet"))
            //{
            //    t.Start();
            //    _sheet = AssemblyViewUtils.CreateSheet(doc, Assembly.AssemblyInstance.Id, formData.SelectedTitleBlockId);
            //    _sheet.Name = Assembly.AssemblyInstance.Name;
            //    //_sheet.LookupParameter("Drawn By")?.Set(rch.userName);

            //    //double spacing = 0.01;
            //    var sheetProxy = new Box2d(_sheet.Outline.Min.AsVector2(), _sheet.Outline.Max.AsVector2());

            //    // quadrant 2 - Top Right
            //    placeSchedule();
            //    var bom = ScheduleSheetInstance.ExportImage(doc, _sheet.Id, Assembly.BillOfMaterials.Id, new XYZ(0, 0, 0));

            //    ISpoolSheetDefinition sheetDefinition = formData.ViewPort;
            //    var bomWidth = sheetDefinition.GetBOMWidth();
            //    var bomPoint = sheetDefinition.GetBOMInsertionPoint(_sheet.Outline.Max.U, _sheet.Outline.Max.V);
            //    bom.Point = new XYZ(bomPoint.X, bomPoint.Y, bomPoint.Z);

            //    double centerX = 0;
            //    double centerY = 0.4;
            //    double centerZ = 0;

            //    double sheetWidth = _sheet.Outline.Max.U - _sheet.Outline.Min.U;

            //    // quadrant 1 - Top Left
            //    var q1View = Assembly.Views[0];
            //    double availableWidth = sheetWidth - bomWidth;
            //    double requiredWidth = (q1View.Outline.Max.U - q1View.Outline.Min.U) * q1View.Scale + 2.0 * spacing;
            //    int q1Scale = (int)Geometry.Ceiling(requiredWidth / availableWidth);
            //    q1View.Scale = q1Scale;

            //    Viewport vp1 = Viewport.ExportImage(doc, _sheet.Id, q1View.Id, new XYZ(centerX, centerY, centerZ));
            //    var vpOutline1 = vp1.GetBoxOutline();
            //    double lenX1 = vpOutline1.MaximumPoint.X
            //                 - vpOutline1.MinimumPoint.X;
            //    double lenY1 = vpOutline1.MaximumPoint.Y
            //                 - vpOutline1.MinimumPoint.Y;

            //    var centerY1 = _sheet.Outline.Max.V - (lenY1 / 2.0);
            //    centerX = (lenX1 / 2.0) + spacing;
            //    vp1.SetBoxCenter(new XYZ(centerX, centerY1, centerZ));


            //    // quadrants 3 and 4 - Bottom

            //    double totalViewWidth = Assembly.Views.Skip(1).Sum(x => (x.Outline.Max.U - x.Outline.Min.U) * x.Scale);
            //    int scale = (int)Geometry.Ceiling(totalViewWidth / sheetWidth);

            //    double lastX = 0;
            //    foreach (var view in Assembly.Views.Skip(1))
            //    {
            //        view.Scale = scale;
            //        Viewport vp = Viewport.ExportImage(doc, _sheet.Id, view.Id, new XYZ(centerX, centerY, centerZ));
            //        var vpOutline = vp.GetBoxOutline();
            //        double lenX = vpOutline.MaximumPoint.X
            //                    - vpOutline.MinimumPoint.X;
            //        centerX = lastX + lenX / 2 + spacing;
            //        vp.SetBoxCenter(new XYZ(centerX, centerY, centerZ));
            //        lastX = centerX + lenX / 2;
            //    }



            //    t.Commit();
            //}
        }

        public void PlaceView(ViewSheet sheet, AssemblyMgrView view)
        {
            var doc = sheet.Document;
            //var assembly = doc.GetElement(sheet.AssociatedAssemblyInstanceId);
            if ((view.View is ViewSchedule schedule))
            {
                var def = view.Definition as ViewPortSchedule;
                var unscaledOrigin = view.Definition.Outline.GetCoordinate(def.DockPoint).AsXYZ(0);
                var origin = new XYZ(unscaledOrigin.X * sheet.Outline.Max.U, unscaledOrigin.Y * sheet.Outline.Max.U, 0);

                // insert the schedule first, since it doesn't have a size until it's created
                var bom = ScheduleSheetInstance.Create(doc, sheet.Id, schedule.Id, new XYZ(0, 0, 0));

                var bbox = bom.get_BoundingBox(sheet);
                var scheduleOutline = new Box2d((bbox.Min.X, bbox.Min.Y), (bbox.Max.X, bbox.Max.Y));
                var translation = origin - scheduleOutline.GetCoordinate(def.DockPoint).AsXYZ(0);

                // Schedules insert point is top-left
                bom.Point = bom.Point + translation;
            }
            else
            {
                // View insert poitn is the center
                var sheetOutline = sheet.Outline.AsBox2d();
                var viewOutline = view.View.Outline.AsBox2d();

                var unscaledOrigin = view.Definition.Outline.Center.AsXYZ(0);
                var origin = new XYZ(unscaledOrigin.X * sheet.Outline.Max.U, unscaledOrigin.Y * sheet.Outline.Max.U, 0);

                var desiredWidth = view.Definition.Outline.Width * sheetOutline.Width;
                var actualWidth = viewOutline.Width * view.View.Scale;
                var desiredHeight = view.Definition.Outline.Height * sheetOutline.Height;
                var actualHeight = viewOutline.Height * view.View.Scale;

                var scale = Math.Max(actualWidth / desiredWidth, actualHeight / desiredHeight);
                view.View.Scale = (int)scale;

                var viewport = Viewport.Create(doc, sheet.Id, view.View.Id, origin);

                //var width = viewport.GetBoxOutline().MaximumPoint.Y - viewport.GetBoxOutline().MinimumPoint.Y;

                //viewport.SetBoxCenter(origin);
            }
        }

        private void placeSchedule()
        {
            throw new NotImplementedException();
        }
    }
}
