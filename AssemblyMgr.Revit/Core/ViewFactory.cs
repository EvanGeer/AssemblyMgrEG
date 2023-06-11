using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Extensions;
using AssemblyMgr.Revit.Data;
using AssemblyMgr.Revit.Extensions;
using Autodesk.Revit.DB;

namespace AssemblyMgr.Revit.Core
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

        /// <summary>Ouput Bill of Materials schedule view</summary>
        public ViewSchedule BillOfMaterials { get; private set; }

        /// <summary>Form interface implementation</summary>
        public ScheduleData ScheduleData => Adapter.BomDefintion;

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

        public List<AssemblyMgrView> CreateViews(IEnumerable<ViewPortDefinition> viewDefinitions)
        {
            var assemblyMgrViews = viewDefinitions
                .Select(x => createView(x))
                .OfType<AssemblyMgrView>()
                .ToList();

            // ToDo: this feels like it should be in an annotation factory
            var viewsToTag = assemblyMgrViews
                .Where(x => x.Definition is ViewPortModel def && def.HasTags)
                .ToList();

            viewsToTag.ForEach(x => TagAllPipeElements(x));

            return assemblyMgrViews;
        }

        private AssemblyMgrView createView(ViewPortDefinition viewPort)
        {
            switch (viewPort.Type)
            {
                case ViewPortType.Schedule:
                    //return CreateBillOfMaterials(viewPort as ViewPortCustomBom);
                    return CreateSchedule(viewPort as ViewPortSchedule);
                case ViewPortType.ModelElevation:
                case ViewPortType.ModelPlan:
                    return Create2DView(viewPort as ViewPortModel);
                case ViewPortType.ModelOrtho:
                    return Create3DView(viewPort as ViewPortModel);
                case ViewPortType.None:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates and commits 3D view
        /// </summary>
        private AssemblyMgrView Create3DView(ViewPortModel viewDefinition)
        {
            if (viewDefinition is null || viewDefinition.Type != ViewPortType.ModelOrtho) return null;

            var view = AssemblyViewUtils.Create3DOrthographic(Doc, AssemblyInstance.Id);
            view.SaveOrientationAndLock();

            return new AssemblyMgrView(viewDefinition, view);
        }

        /// <summary>
        /// Tags all elements in a 3D view.
        /// </summary>
        /// <param name="view">View to tag</param>
        /// <param name="tagOffset">Tag off set if using leaders. Will be 0 if leaders are not used.</param>
        public void TagAllPipeElements(AssemblyMgrView assemblyMgrView)
        {
            if (!(assemblyMgrView.Definition is ViewPortModel viewDef)) return;
            TagAllPipeElements(assemblyMgrView.View, false);
        }
        public void TagAllPipeElements(View view, bool IgnoreWelds)
        {
            //get all elems in view via filtered element collector
            var fec = new FilteredElementCollector(Doc, view.Id);
            fec.OfCategory(BuiltInCategory.OST_FabricationPipework);

            // ToDo: custom tag offset per tag type
            var tagOffset = 6 / 12.0; // viewDef.TagOffset; 


            //pair these down to just the stuff we're interested in
            // ToDo: 
            // - Straights = location is Curve
            // - Joints & Fittings = everything else
            IEnumerable<Element> elements;
            if (IgnoreWelds)
            {
                elements = fec
                    .Where(x => null != x.LookupParameter("Product Short Description"))
                    .Where(x => !x.LookupParameter("Product Short Description").AsString().Contains("weld", StringComparison.InvariantCultureIgnoreCase));
                if (elements.Count() == 0)
                    elements = fec.Select(x => x);
            }
            else
                elements = fec.Select(x => x);

            //build out tags
            if (view is View3D view3d && !view3d.IsLocked)
                view3d.SaveOrientationAndLock();

            foreach (var elem in elements.OfType<FabricationPart>())
            {
                var elemRef = new Reference(elem);
                var elementCenter = elem.get_BoundingBox(view).GetCenter();
                var connectors = elem.ConnectorManager.Connectors
                    .OfType<Connector>()
                    .Where(x => x.ConnectorType == ConnectorType.End);

                var elementDirection = connectors
                    .Select(x => x.CoordinateSystem.BasisZ)
                    .Aggregate((x, y) => x + y);

                // this should be normal to the element
                var offsetDiretion = elementDirection.IsAlmostEqualTo(XYZ.Zero)
                    ? connectors.FirstOrDefault()?.CoordinateSystem.BasisZ.CrossProduct(view.ViewDirection)
                    : elementDirection.Negate();

                var tagXYZ = elementCenter + offsetDiretion * tagOffset;

                var tagTypes = new FilteredElementCollector(Doc)
                    .OfCategory(BuiltInCategory.OST_FabricationPipeworkTags)
                    .OfClass(typeof(FamilySymbol))
                    .ToElements();

                var tagType = tagTypes.FirstOrDefault();

                var tag = IndependentTag.Create(Doc, tagType.Id, view.Id, elemRef, true/* viewDef.HasTagLeaders*/, TagOrientation.Horizontal, tagXYZ);
                tag.LeaderEndCondition = LeaderEndCondition.Free;
                tag.SetLeaderEnd(elemRef, elementCenter);
                tag.TagHeadPosition = tagXYZ;
            }

        }

        private AssemblyMgrView CreateSchedule(ViewPortSchedule viewDefinition)
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

                return new AssemblyMgrView(viewDefinition, billOfMaterials);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// Creates bill of materials per specification in the FormData object field
        /// </summary>
        private AssemblyMgrView CreateBillOfMaterials(ViewPortCustomBom viewDefinition)
        {
            if (viewDefinition is null) return null;

            //assume that all elements are the same category for now
            var categoryId = Doc.GetElement(AssemblyInstance.GetMemberIds().First()).Category.Id;

            //ExportImage the schedul using the built-in API util
            var billOfMaterials = AssemblyViewUtils
                    .CreateSingleCategorySchedule(Doc, AssemblyInstance.Id, categoryId);

            billOfMaterials.Name = AssemblyInstance.Name + " - Bill of Materials";
            billOfMaterials.Definition.ClearFields();

            AddColumnsToSchedule(billOfMaterials, viewDefinition);

            if (viewDefinition.IgnoreWelds)
                AddScheduleWeldFilter(billOfMaterials);

            BillOfMaterials = billOfMaterials;

            return new AssemblyMgrView(viewDefinition, billOfMaterials);
        }

        private void AddColumnsToSchedule(ViewSchedule billOfMaterials, ViewPortCustomBom viewDefinition)
        {
            //add user fields to schedule with custom headers and widths
            // ToDo: make sure that we're building the BOMFields correctly
            foreach (var fieldDef in viewDefinition.BOMFields.OfType<BOMFieldDefintion_Revit>()) // extra work to grab param, but will preserve users' ordering
            {
                var field = fieldDef.SchedulableField;

                if (field == null) continue;

                var newField = billOfMaterials.Definition.AddField(field);
                newField.ColumnHeading = fieldDef.ColumnHeader;
                newField.SheetColumnWidth = fieldDef.ColumnWidth;
                newField.GridColumnWidth = fieldDef.ColumnWidth;
            }
        }

        private void AddScheduleWeldFilter(ViewSchedule billOfMaterials)
        {
            string filterFieldName = "Product Short Description"; // there may be a better parameter to use. This could be added to the form as well.

            //add hidden filter field if the user didn't select it
            var filterField =
                GetFieldFromSchedule(billOfMaterials, filterFieldName)
                ?? AddHiddenField(billOfMaterials, filterFieldName);

            //apply filter
            var weldFilter = new ScheduleFilter(filterField.FieldId, ScheduleFilterType.NotContains, "Weld");
            billOfMaterials.Definition.AddFilter(weldFilter);
        }

        private ScheduleField GetFieldFromSchedule(ViewSchedule billOfMaterials, string filterFieldName)
        {
            for (int i = 0; i < billOfMaterials.Definition.GetFieldCount(); i++)
            {
                var curnetField = billOfMaterials.Definition.GetField(i);
                if (curnetField.GetName() == filterFieldName) return curnetField;
            }

            return null; // fail case
        }

        private ScheduleField AddHiddenField(ViewSchedule billOfMaterials, string filterFieldName)
        {
            ScheduleField filterField;
            var newFilterField = ScheduleData.SchedulableFields
                .FirstOrDefault(x => x.GetName(Doc) == filterFieldName);

            filterField = billOfMaterials.Definition.AddField(newFilterField);
            filterField.IsHidden = true;
            return filterField;
        }

        /// <summary>
        /// Creates 2D view of selected orientation and commits to model.
        /// </summary>
        /// <param name="orientation">Selected orientation of 2D view</param>
        public AssemblyMgrView Create2DView(ViewPortModel viewDefinition)
        {
            if (viewDefinition == null) return null;
            var orientation = viewDefinition.Type == ViewPortType.ModelElevation
                ? viewDefinition.Direction.AsAssemblyDetailViewOrientation()
                : AssemblyDetailViewOrientation.ElevationTop;

            var view = AssemblyViewUtils.CreateDetailSection(Doc, AssemblyInstance.Id, orientation);

            var adapter = new ViewTemplateController(Doc);
            var template = adapter.GetModelViewTemplate(viewDefinition.ViewTemplate);
            if (!(template is null) && template.Id != ElementId.InvalidElementId)
                view.ViewTemplateId = template.Id;

            return new AssemblyMgrView(viewDefinition, view);
        }


        /// <summary>
        /// Out of time on this one ... see a future release 
        /// </summary>
        /// <remarks>
        /// I haven't done a ton with fabrication part annotations, and this one will require
        /// some research on my part. 
        /// </remarks>
        /// <param name="view"></param>
        public void DimensionAllElements(ViewSection view)
        {

            //    var fec = new FilteredElementCollector(_doc, view.Id);
            //    fec.OfCategory(BuiltInCategory.OST_FabricationPipework);

            //    using (Transaction t = new Transaction(_doc, "Add Dimension"))
            //    {
            //        t.Start();


            //        foreach (var elem in fec)
            //        {
            //            var part = elem as FabricationPart;
            //            var loc = part.Location;
            //            var dims = part.;
            //            foreach (var dim in dims)
            //            {
            //                dim.
            //            }


            //            var refs = new ReferenceArray();
            //            refs.Append(loc.Curve.GetEndPointReference(0));
            //            refs.Append(loc.Curve.GetEndPointReference(1));

            //            XYZ startPoint = refs.get_Item(0).GlobalPoint;
            //            XYZ endPoint = refs.get_Item(1).GlobalPoint;
            //            Line dimLine = Line.CreateBound(startPoint, endPoint);



            //            //refs.Append(elem.get_Geometry().GetBoundingBox()..GeometryCurve.GetEndPointReference(0));
            //            //refs.Append(part.GeometryCurve.GetEndPointReference(1));

            //            //XYZ startPoint = refs.get_Item(0).GlobalPoint;
            //            //XYZ endPoint = refs.get_Item(1).GlobalPoint;
            //            //Line dimLine = Line.CreateBound(startPoint, endPoint);

            //            _doc.ExportImage.NewDimension(view, dimLine, refs);
            //        }
            //        t.Commit();
            //}
        }

    }
}
