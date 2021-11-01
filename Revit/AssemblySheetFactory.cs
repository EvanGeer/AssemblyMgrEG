using System.Collections.Generic;
using System.Linq;
using AssemblyManagerUI.DataModel;
using AssemblyManagerUI.TestData;
using AssemblyMgrRevit.Data;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AssemblyMgrEG.Revit
{
    /// <summary>
    /// Assembly wrapper class for Assembly Manager Add-In.
    /// Contains data and methods required to create manipulate assemblies
    /// </summary>
    public class AssemblySheetFactory
    {
        private RevitCommandHelper rch { get; set; }

        public SpoolSheetDefinition SpoolSheetDefinition { get; }

        /// <summary>
        /// Current Revit Document Object
        /// shorthand, makes for cleaner code and less horizontal scroll
        /// </summary>
        private Document doc { get => rch.ActiveDoc; }

        /// <summary>
        /// Revit Assembly object
        /// </summary>
        public AssemblyInstance AssemblyInstance { get; private set; }

        /// <summary>
        /// Output list of Assembly View objects created by this tool
        /// </summary>
        public List<View> Views { get; private set; } = new List<View>();
        
        /// <summary>
        /// Ouput Bill of Materials schedule view
        /// </summary>
        public ViewSchedule BillOfMaterials { get; private set; }
    
        /// <summary>
        /// Form interface implementation
        /// </summary>
        public AssemblyMgrDataModel AssemblyDataModel { get; set; }

        /// <summary>
        /// Main Constructor - Builds new assembly from selected geometry
        /// </summary>
        /// <param name="helper">Simple helper class to keep API references cleaner</param>
        /// <param name="BomFields">In case we want to customize the defaults at app startup</param>
        public AssemblySheetFactory(RevitCommandHelper helper)
        {
            rch = helper;
            AssemblyInstance = GetAssemblyInstance();
            SpoolSheetDefinition = new SpoolSheetDefinition()
            {
                AssemblyName = AssemblyInstance?.Name
            };
            AssemblyDataModel = new AssemblyMgrDataModel(SpoolSheetDefinition, AssemblyInstance);
        }



        /// <summary>
        /// Creates and commits 3D view
        /// </summary>
        public void Create3DView()
        {
            View3D view;
            bool viewCreated = false;
            using (Transaction t = new Transaction(doc, "Assembly Manager: Create 3D View"))
            {
                t.Start();
                view = AssemblyViewUtils.Create3DOrthographic(rch.ActiveDoc, AssemblyInstance.Id);
                view.SaveOrientationAndLock();

                Views.Add(view);

                viewCreated = t.Commit() == TransactionStatus.Committed;

            }
            if (viewCreated)
                TagAllPipeElements(view);
        }

        /// <summary>
        /// Tags all elements in a 3D view.
        /// </summary>
        /// <param name="view">View to tag</param>
        /// <param name="tagOffset">Tag off set if using leaders. Will be 0 if leaders are not used.</param>
        public void TagAllPipeElements(View3D view, double tagOffset = 1.5)
        {
            if (AssemblyDataModel.SpoolSheetDefinition.TagLeaders == false)
                tagOffset = 0;
            
            //get all elems in view via filtered element collector
            var fec = new FilteredElementCollector(doc, view.Id);
            fec.OfCategory(BuiltInCategory.OST_FabricationPipework);

            //pair these down to just the stuff we're interested in
            IEnumerable<Element> elements;
            if (AssemblyDataModel.SpoolSheetDefinition.IgnoreWelds)
            {
                elements = fec
                    .Where(x => null != x.LookupParameter("Product Short Description"))
                    .Where(x => !x.LookupParameter("Product Short Description").AsString().ToLower().Contains("weld"));
                if (elements.Count() == 0)
                    elements = fec.Select(x => x);
            }
            else
                elements = fec.Select(x => x);
            
            //build out tags
            using (Transaction t = new Transaction(doc, "Create Tags"))
            {
                t.Start();

                if (!view.IsLocked)
                    view.SaveOrientationAndLock();

                foreach (var elem in elements)
                {
                    var elemRef = new Reference(elem);
                    var fabPipeElem = (FabricationPart)elem;
                    var maxXYZ = elem.get_BoundingBox(view).Max;
                    var minXYZ = elem.get_BoundingBox(view).Min;
                    var tagXYZ = new XYZ(
                        ((maxXYZ.X + minXYZ.X) / 2.0) + (tagOffset),
                        ((maxXYZ.Y + minXYZ.Y) / 2.0) + (tagOffset),
                        ((maxXYZ.Z + minXYZ.Z) / 2.0) + (tagOffset)
                    );
                    IndependentTag.Create(doc, view.Id, elemRef, tagOffset != 0, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, tagXYZ);
                }

                t.Commit();
            }
        }

        /// <summary>
        /// Creates bill of materials per specification in the FormData object field
        /// </summary>
        public void CreateBillOfMaterials()
        {
            //assume that all elements are the same category for now
            var categoryId = doc.GetElement(AssemblyInstance.GetMemberIds().First()).Category.Id;

            using (Transaction t = new Transaction(doc, "Assembly Manager: Create Bill of Materials"))
            {
                t.Start();

                //Create the schedul using the built-in API util
                ViewSchedule billOfMaterials = AssemblyViewUtils
                    .CreateSingleCategorySchedule(rch.ActiveDoc, AssemblyInstance.Id, categoryId);

                billOfMaterials.Name = AssemblyInstance.Name + " - Bill of Materials";
                billOfMaterials.Definition.ClearFields();

                AddColumnsToSchedule(billOfMaterials);

                if (AssemblyDataModel.SpoolSheetDefinition.IgnoreWelds)
                    AddScheduleWeldFilter(billOfMaterials);

                BillOfMaterials = billOfMaterials;
                t.Commit();
            }
        }

        private void AddColumnsToSchedule(ViewSchedule billOfMaterials)
        {
            //add user fields to schedule with custom headers and widths
            foreach (var fieldDef in AssemblyDataModel.SpoolSheetDefinition.BOMFields.OfType<RevitFieldDefintion>()) // extra work to grab param, but will preserve users' ordering
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
            var newFilterField = AssemblyDataModel
                .BomDefintion
                .SchedulableFields
                .FirstOrDefault(x => x.GetName(doc) == filterFieldName);

            filterField = billOfMaterials.Definition.AddField(newFilterField);
            filterField.IsHidden = true;
            return filterField;
        }

        /// <summary>
        /// Creates 2D view of selected orientation and commits to model.
        /// </summary>
        /// <param name="orientation">Selected orientation of 2D view</param>
        public void Create2DView(AssemblyDetailViewOrientation orientation)
        {
            var tranactionName = string.Format("Assembly Manager: Create {0} View", orientation.ToString());
            ViewSection view;
            using (Transaction t = new Transaction(doc, tranactionName))
            {
                t.Start();
                view = AssemblyViewUtils.CreateDetailSection(rch.ActiveDoc, AssemblyInstance.Id, orientation);
                Views.Add(view);

                t.Commit();
            }
            // DimensionAllElements(view);
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

            //    var fec = new FilteredElementCollector(doc, view.Id);
            //    fec.OfCategory(BuiltInCategory.OST_FabricationPipework);

            //    using (Transaction t = new Transaction(doc, "Add Dimension"))
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

            //            doc.Create.NewDimension(view, dimLine, refs);
            //        }
            //        t.Commit();
            //}
        }

        /// <summary>
        /// Creates Assembly Instance object for manipulation
        /// If the selected object is not already an Assembly, an assembly would be created. 
        /// </summary>
        /// <remarks>
        /// This may be a good application for PostCommand. The benefit of using the built-in command here is that
        /// you would get element validation for "free" as well as the more intuitive interactions for the user.
        /// My typical approach is to try to disturb the user's typical workflow as little as possible, but the 
        /// assigned task here asked us to do everything via code, and PostCommand feels a bit like cheating in 
        /// this particular case. 8-)
        /// </remarks>
        /// <returns>AssemblyInstance</returns>
        private AssemblyInstance GetAssemblyInstance(List<ElementId> elemIds = null)
        {
            /// To-Do: 
            ///     Check valid content
            ///     If content is already select, don't enact the pick box
            ///     Right now this will only allow you select multiple items to build out a new assembly
            ///     if you click before select or if you pick an invalid selection to start.
            ///     Ideally we would create some better logic and/or simple form controls to figure out 
            ///     what the user is intending to do. 

            AssemblyInstance assembly;
            var selection = rch.UiDoc.Selection;
            elemIds = elemIds ?? selection.GetElementIds().ToList();

            //List<ElementId> elemIds;
            ElementId categoryId;

            //try
            //{
            //if one item is selected and its an assembly, just use that
            if (elemIds.Count == 1)
            {
                var fec = new FilteredElementCollector(doc, elemIds);
                fec.OfCategory(BuiltInCategory.OST_Assemblies);

                if (fec.Count() == 1)
                    return (AssemblyInstance)fec.First();
            }

            //if multiple items are selected and they can be made into an assembly, build an assembly
            else if (elemIds.Count > 0)
            {
                //elemIds = selection.GetElementIds().ToList();
                categoryId = doc.GetElement(elemIds.FirstOrDefault()).Category.Id;
                try
                {
                    using (Transaction t = new Transaction(doc, "Assembly Manager: Create Assembly"))
                    {
                        t.Start();
                        assembly = AssemblyInstance.Create(doc, elemIds, categoryId);
                        if (t.Commit() == TransactionStatus.Committed)
                            return assembly;
                    }
                }
                catch
                {
                    TaskDialog.Show("Invalid Selection",
                        "Invalid Selection. Please select either:" +
                        "\n\n\t- A single Assembly object" +
                        "\n\t- A set of valid objects to build and assembly");

                    //To-Do: add better validatation for trying to create an assembly from selection
                }
            }

            //else nothing valid is chosen, as the user to select and run then method again.
            var elems = selection.PickElementsByRectangle();
            elemIds = elems.Select(x => x.Id).ToList();
            
            //if nothing is selected or user cancels, cancel out
            if (elems.Count == 0)
                return null;

            //otherwise, try again recursively
            return GetAssemblyInstance(elemIds);
        }

    }
}
