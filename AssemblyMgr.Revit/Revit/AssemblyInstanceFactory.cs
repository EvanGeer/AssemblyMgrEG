using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AssemblyMgrEG.Revit
{
    public class AssemblyInstanceFactory
    {
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
        public static AssemblyInstance CreateBySelection(UIDocument uiDoc, List<ElementId> elemIds = null)
        {
            /// To-Do: 
            ///     Check valid content
            ///     If content is already selected, don't enact the pick box
            ///     Right now this will only allow you select multiple items to build out a new assembly
            ///     if you click before select or if you pick an invalid selection to start.
            ///     Ideally we would create some better logic and/or simple form controls to figure out 
            ///     what the user is intending to do. 

            AssemblyInstance assembly;
            var selection = uiDoc.Selection;
            var Doc = uiDoc.Document;

            elemIds = elemIds ?? selection.GetElementIds().ToList();

            //List<ElementId> elemIds;
            ElementId categoryId;

            //try
            //{
            //if one item is selected and its an assembly, just use that
            if (elemIds.Count == 1)
            {
                var fec = new FilteredElementCollector(Doc, elemIds);
                fec.OfCategory(BuiltInCategory.OST_Assemblies);

                if (fec.Count() == 1)
                    return (AssemblyInstance)fec.First();
            }

            //if multiple items are selected and they can be made into an assembly, build an assembly
            else if (elemIds.Count > 0)
            {
                //elemIds = selection.GetElementIds().ToList();
                categoryId = Doc.GetElement(elemIds.FirstOrDefault()).Category.Id;
                try
                {
                    using (Transaction t = new Transaction(Doc, "Assembly Manager: Create Assembly"))
                    {
                        t.Start();
                        assembly = AssemblyInstance.Create(Doc, elemIds, categoryId);
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

            //else nothing valid is chosen, ask the user to select and run then method again.
            var elems = selection.PickElementsByRectangle();
            elemIds = elems.Select(x => x.Id).ToList();

            //if nothing is selected or user cancels, cancel out
            if (elems.Count == 0)
                return null;

            //otherwise, try again recursively
            return CreateBySelection(uiDoc, elemIds);
        }
    }
}
