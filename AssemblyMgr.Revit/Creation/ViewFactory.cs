using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Extensions;
using AssemblyMgr.Revit.DataExtraction;
using AssemblyMgr.Revit.Extensions;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyMgr.Revit.Creation
{
    /// <summary>
    /// Assembly wrapper class for Assembly Manager Add-In.
    /// Contains data and methods required to create manipulate assemblies
    /// </summary>
    public class ViewFactory
    {
        /// <summary>
        /// Current Revit__Core Document Object
        /// shorthand, makes for cleaner code and less horizontal scroll
        /// </summary>
        private Document Doc { get; }

        /// <summary>
        /// Revit__Core Assembly object
        /// </summary>
        public AssemblyInstance AssemblyInstance { get; private set; }
        public AssemblyMangerRevitAdapter Adapter { get; }

        /// <summary>Main Constructor - Builds new Assembly from selected geometry</summary>
        /// <param name="helper">Simple helper class to keep API references cleaner</param>
        /// <param name="BomFields">In case we want to customize the defaults at app startup</param>
        public ViewFactory(AssemblyInstance assembly, AssemblyMangerRevitAdapter adapter)
        {
            //AssemblyDataModel = assemblyDataModel;
            AssemblyInstance = assembly;
            Adapter = adapter;
            Doc = assembly.Document;
        }

        public List<BuiltViewPort> CreateViews(IEnumerable<ViewPortDefinition> viewDefinitions)
        {
            var assemblyMgrViews = viewDefinitions
                .Select(x => createView(x))
                .OfType<BuiltViewPort>()
                .ToList();

            return assemblyMgrViews;
        }

        private BuiltViewPort createView(ViewPortDefinition viewPort)
        {
            switch (viewPort.Type)
            {
                case ViewPortType.Schedule:
                    //return CreateBillOfMaterials(viewPort as ViewPortCustomBom);
                    return CreateSchedule(viewPort as ViewPortDefinition_Schedule);
                case ViewPortType.ModelElevation:
                case ViewPortType.ModelPlan:
                    return Create2DView(viewPort as ViewPortDefinition_Model);
                case ViewPortType.ModelOrtho:
                    return Create3DView(viewPort as ViewPortDefinition_Model);
                case ViewPortType.None:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates and commits 3D view
        /// </summary>
        private BuiltViewPort Create3DView(ViewPortDefinition_Model viewDefinition)
        {
            if (viewDefinition is null || viewDefinition.Type != ViewPortType.ModelOrtho) return null;

            var template = Adapter.GetViewTemplate(viewDefinition.ViewTemplate);
            var view = AssemblyViewUtils.Create3DOrthographic(Doc, AssemblyInstance.Id, template.Id, true);
            view.SaveOrientationAndLock();

            return new BuiltViewPort(viewDefinition, view);
        }


        private BuiltViewPort CreateSchedule(ViewPortDefinition_Schedule viewDefinition)
        {
            if (viewDefinition is null) return null;

            try
            {
                var template = Adapter.GetScheduleTemplate(viewDefinition.ViewTemplate);
                var billOfMaterials = AssemblyViewUtils.CreateSingleCategorySchedule(
                    Doc,
                    AssemblyInstance.Id,
                    template.Definition.CategoryId,
                    template.Id,
                    isAssigned: true);

                return new BuiltViewPort(viewDefinition, billOfMaterials);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// Creates 2D view of selected orientation and commits to model.
        /// </summary>
        /// <param name="orientation">Selected orientation of 2D view</param>
        public BuiltViewPort Create2DView(ViewPortDefinition_Model viewDefinition)
        {
            if (viewDefinition == null) return null;
            var orientation = viewDefinition.Type == ViewPortType.ModelElevation
                ? viewDefinition.Direction.AsAssemblyDetailViewOrientation()
                : AssemblyDetailViewOrientation.ElevationTop;

            var view = AssemblyViewUtils.CreateDetailSection(Doc, AssemblyInstance.Id, orientation);

            var adapter = new ViewTemplateExtractor(Doc);
            var template = adapter.GetModelViewTemplate(viewDefinition.ViewTemplate);
            if (!(template is null) && template.Id != ElementId.InvalidElementId)
                view.ViewTemplateId = template.Id;

            return new BuiltViewPort(viewDefinition, view);
        }
    }
}
