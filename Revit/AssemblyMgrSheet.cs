using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyMgrRevit.Data;
using Autodesk.Revit.DB;

namespace AssemblyMgrEG.Revit
{
    public class AssemblyMgrSheet
    {
        public ViewSheet Sheet { get => sheet; }
        private ViewSheet sheet { get; set; }

        private AssemblyMgrDataModel formData;
        private ViewFactory assembly;
        private Document Doc;
        
        public AssemblyMgrSheet(ViewFactory Assembly)
        {
            Doc = Assembly.AssemblyDataModel.Doc;
            formData = Assembly.AssemblyDataModel;
            assembly = Assembly;
            CreateSheet();
        }

        private void CreateSheet()
        {
            using (Transaction t = new Transaction(Doc, "Assembly Manager: Create Sheet"))
            {
                t.Start();
                sheet = AssemblyViewUtils.CreateSheet(Doc, assembly.AssemblyInstance.Id, formData.SelectedTitleBlockId);
                sheet.Name = assembly.AssemblyInstance.Name;
                //sheet.LookupParameter("Drawn By")?.Set(rch.userName);

                //To-Do parameterize sheet sizing
                //assume sheet is 11x17
                double spacing = 0.01;

                if (null != assembly.BillOfMaterials)
                {
                    var bom = ScheduleSheetInstance.Create(Doc, sheet.Id, assembly.BillOfMaterials.Id, new XYZ(0, 0, 0));
                    var len = assembly.AssemblyDataModel.SpoolSheetDefinition.BOMFields.Sum(x => x.ColumnWidth);
                    var bomX = 17.0 / 12.0 - len - spacing;
                    var bomY = 11.0 / 12.0 - spacing;
                    var bomZ = 0;
                    bom.Point = new XYZ(bomX, bomY, bomZ);
                }

                //To-Do: Clean up layout stuff, probably could parameterize all this and put in form and/or add some intelligence
                double centerX = 0;
                double centerY = 0.4;
                double centerZ = 0;

                double lastX = 0;

                foreach (var view in assembly.Views)
                {
                    view.Scale = 48; //could be parameterized
                    Viewport vp = Viewport.Create(Doc, sheet.Id, view.Id, new XYZ(centerX, centerY, centerZ));
                    double lenX = vp.GetBoxOutline().MaximumPoint.X - vp.GetBoxOutline().MinimumPoint.X;
                    centerX = lastX + lenX / 2 + spacing;
                    vp.SetBoxCenter(new XYZ(centerX, centerY, centerZ));
                    lastX = centerX + lenX / 2;
                    //i++;
                }


                t.Commit();
            }
        }
    }
}
