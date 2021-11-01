﻿using System;
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
            // To-Do: Clean up layout stuff, probably could parameterize all this and put in form and/or add some intelligence
            // To-Do: Break this out into the business logic sessoin and cover the functionality with unit tests

            using (Transaction t = new Transaction(Doc, "Assembly Manager: Create Sheet"))
            {
                t.Start();
                sheet = AssemblyViewUtils.CreateSheet(Doc, assembly.AssemblyInstance.Id, formData.SelectedTitleBlockId);
                sheet.Name = assembly.AssemblyInstance.Name;
                //sheet.LookupParameter("Drawn By")?.Set(rch.userName);

                double spacing = 0.01;

                // quadrant 2 - Top Right
                var bom = ScheduleSheetInstance.Create(Doc, sheet.Id, assembly.BillOfMaterials.Id, new XYZ(0, 0, 0));
                var bomWidth = formData.SpoolSheetDefinition.BOMFields.Sum(x => x.ColumnWidth);
                var bomX = sheet.Outline.Max.U - bomWidth - spacing;
                var bomY = sheet.Outline.Max.V - spacing;
                var bomZ = 0;
                bom.Point = new XYZ(bomX, bomY, bomZ);

                double centerX = 0;
                double centerY = 0.4;
                double centerZ = 0;

                double lastX = 0;
                double sheetWidth = sheet.Outline.Max.U - sheet.Outline.Min.U;

                // quadrant 1 - Top Left
                var q1View = assembly.Views[0];
                double availableWidth = sheetWidth - bomWidth;
                double requiredWidth = (q1View.Outline.Max.U - q1View.Outline.Min.U) * q1View.Scale + 2.0 * spacing;
                int q1Scale = (int)Math.Ceiling(requiredWidth / availableWidth);
                q1View.Scale = q1Scale;

                Viewport vp1 = Viewport.Create(Doc, sheet.Id, q1View.Id, new XYZ(centerX, centerY, centerZ));
                var vpOutline1 = vp1.GetBoxOutline();
                double lenX1 = vpOutline1.MaximumPoint.X
                             - vpOutline1.MinimumPoint.X;
                double lenY1 = vpOutline1.MaximumPoint.Y
                             - vpOutline1.MinimumPoint.Y;

                var centerY1 = sheet.Outline.Max.V - (lenY1 / 2.0);
                centerX = (lenX1 / 2.0) + spacing;
                vp1.SetBoxCenter(new XYZ(centerX, centerY1, centerZ));
                
                
                // quadrants 3 and 4 - Bottom

                double totalViewWidth = assembly.Views.Skip(1).Sum(x => (x.Outline.Max.U - x.Outline.Min.U) * x.Scale);
                int scale = (int)Math.Ceiling(totalViewWidth / sheetWidth);

                foreach (var view in assembly.Views.Skip(1))
                {
                    view.Scale = scale;
                    Viewport vp = Viewport.Create(Doc, sheet.Id, view.Id, new XYZ(centerX, centerY, centerZ));
                    var vpOutline = vp.GetBoxOutline();
                    double lenX = vpOutline.MaximumPoint.X
                                - vpOutline.MinimumPoint.X;
                    centerX = lastX + lenX / 2 + spacing;
                    vp.SetBoxCenter(new XYZ(centerX, centerY, centerZ));
                    lastX = centerX + lenX / 2;
                }



                t.Commit();
            }
        }
    }
}
