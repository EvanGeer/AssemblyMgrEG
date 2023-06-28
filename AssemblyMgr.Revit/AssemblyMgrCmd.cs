using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgr.UI;
using AssemblyMgr.UI.ViewModels;
using AssemblyMgr.Core.DataModel;
using Settings = AssemblyMgr.Core.Settings.Settings;
using System.Collections.Generic;
using AssemblyMgr.Revit.Creation;
using AssemblyMgr.Revit.DataExtraction;
using System.Linq;
using Autodesk.Revit.DB.Plumbing;
using AssemblyMgr.Core.Settings;

namespace AssemblyMgr.Revit.Core
{
    /// <summary>
    /// Main procedural logic for interacting with Revit.Core:
    /// Builds out views and sheets for new or selected Assembly
    /// </summary>
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class AssemblyMgrCmd : ExternalCommandBase
    {
        private AssemblyInstance _assemblyInstance;
        public override Result Execute()
        {
            using (var t = new Transaction(Doc, "Create Assembly"))
            {
                t.Start();

                // build the Assembly from the user selection
                // ToDo: return a collection of assemblies so then we can iterate through those when building sheets
                _assemblyInstance = AssemblyInstanceFactory.CreateBySelection(UiDoc);
                t.Commit();
            }
            if (null == _assemblyInstance)
                return Result.Cancelled;


            // get the data needed for the form and the command
            var spoolSheetDefinition = Settings.DeSerialize<SpoolSheetDefinition>(SettingsFile)
                ?? new SpoolSheetDefinition();
            var revitAdapter = new AssemblyMangerRevitAdapter(_assemblyInstance);
            var viewModel = new AssemblyMgrVM(spoolSheetDefinition, revitAdapter);


            // get input from the user on how to build the Assembly sheet
            var form = new AssemblyMgrForm(viewModel);
            form.ShowDialog();
            if (!form.Run)
                return Result.Cancelled;


            // save the settings
            spoolSheetDefinition.Serialize(SettingsFile);

            ViewSheet sheet;
            List<BuiltViewPort> views;


            using (var t = new Transaction(Doc, $"Build Views"))
            {
                t.Start();

                var viewFactory = new ViewFactory(_assemblyInstance, revitAdapter);
                views = viewFactory.CreateViews(spoolSheetDefinition.ViewPorts);

                t.Commit();
            }

            using (var t = new Transaction(Doc, "Place Annotatoins"))
            {
                t.Start();

                // Annotation Factory Setup
                var elementDistiller = getElementDistiller();

                // Tags
                placeTags(revitAdapter, views, elementDistiller);

                // Dimensions
                placeDimensions(views, elementDistiller);

                t.Commit();
            }

            using (var t = new Transaction(Doc, $"Place Views on Sheet"))
            {
                t.Start();

                sheet = buildSheet(spoolSheetDefinition, revitAdapter, views);

                t.Commit();
            }

            UiDoc.ActiveView = sheet;

            return Result.Succeeded;
        }

        private ViewSheet buildSheet(SpoolSheetDefinition spoolSheetDefinition, AssemblyMangerRevitAdapter revitAdapter, List<BuiltViewPort> views)
        {
            ViewSheet sheet;
            var sheetFactory = new SheetFactory(views);
            var titleBlockId = revitAdapter.GetTitleBlock(spoolSheetDefinition.TitleBlock)?.Id;
            sheet = sheetFactory.CreateSheet(_assemblyInstance, titleBlockId);
            sheetFactory.PlaceViews(sheet);
            return sheet;
        }

        private void placeDimensions(List<BuiltViewPort> views, ElementDistiller distiller)
        {
            var dimFactory = new DimensionFactory(Doc, distiller);
            var viewsToDim = views
                .Where(x => x.Definition is ViewPortDefinition_Model def && def.HasDimensions)
                .ToList();
            viewsToDim.ForEach(x => dimFactory.CreatePipeDimensions(x.View));
        }

        private void placeTags(AssemblyMangerRevitAdapter revitAdapter, List<BuiltViewPort> views, ElementDistiller distiller)
        {
            var tagFactory = new TagFactory(Doc, distiller, revitAdapter.TagTypeController);
            var viewsToTag = views
                .Where(x => x.Definition is ViewPortDefinition_Model def && def.HasTags)
                .ToList();
            viewsToTag.ForEach(x => tagFactory.CreateTags(x));
        }

        private ElementDistiller getElementDistiller()
        {
            var elements = _assemblyInstance.GetMemberIds()
                .Select(x => Doc.GetElement(x))
                .Where(x => x is FamilyInstance || x is FabricationPart || x is Pipe)
                .ToList();
            var distiller = new ElementDistiller(elements);
            return distiller;
        }
    }
}
