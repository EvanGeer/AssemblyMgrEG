using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Revit.Creation;
using AssemblyMgr.Revit.Core;
using AssemblyMgr.Revit.DataExtraction;

namespace AssemblyMgr.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class TestView3DGeneration : ExternalCommandBase
    {
        public override Result Execute()
        {
            // build the Assembly from the user selection
            var currentView = UiDoc.ActiveView;
            AssemblyInstance assemblyInstance = null;
            if (currentView is ViewSheet sheet && sheet.AssociatedAssemblyInstanceId is ElementId id && id != ElementId.InvalidElementId)
                assemblyInstance = Doc.GetElement(id) as AssemblyInstance;
            else assemblyInstance = AssemblyInstanceFactory.CreateBySelection(UiDoc);
            if (null == assemblyInstance)
                return Result.Cancelled;

            // get input from the user on how to build the Assembly _sheet
            var spoolSheetDefinition = new SpoolSheetDefinition();
            spoolSheetDefinition.ViewPorts.Add(new ViewPortDefinition_Model
            {
                Outline = new Box2d((0, 0), (0.5, 0.5)),
                Type = ViewPortType.ModelOrtho,
            });
            spoolSheetDefinition.ViewPorts.Add(new ViewPortDefinition_Model
            {
                Outline = new Box2d((0.5, 0.5), (1, 1)),
                Type = ViewPortType.ModelOrtho,
            });
            var revitAdapter = new AssemblyMangerRevitAdapter(assemblyInstance);

            using (var t = new Transaction(Doc, $"Test Place 3 view"))
            {
                t.Start();

                var viewFactory = new ViewFactory(assemblyInstance, revitAdapter);
                var views = viewFactory.CreateViews(spoolSheetDefinition.ViewPorts);

                var sheetFactory = new SheetFactory(views);
                views.ForEach(x => sheetFactory.PlaceViews(currentView as ViewSheet));

                var P1 = new XYZ(0, 0, 0);
                var P2 = new XYZ(17.0 / 12.0, 11.0 / 12.0, 0);
                var L1 = Line.CreateBound(P1, P2);
                Doc.Create.NewDetailCurve(currentView, L1);

                t.Commit();
            }


            return Result.Succeeded;
        }
    }
}
