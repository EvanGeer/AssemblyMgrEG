using AssemblyMgr.Core.DataModel;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AssemblyMgrRevit.Data
{
    public class ScheduleData
    {
        public Document Doc { get; }
        public AssemblyInstance Assembly { get; }
        public List<SchedulableField> SchedulableFields { get; private set; }
        public List<BOMFieldDefinition> ModelBOMFields { get; private set; }
        public ObservableCollection<BOMFieldDefinition> DefaultFieldList { get; private set; }

        public ScheduleData(Document doc, AssemblyInstance assembly)
        {
            Doc = doc;
            Assembly = assembly;

            initializeScheduleData();
        }

        private void initializeScheduleData()
        {
            SchedulableFields = getBomSchedulableFields();
            ModelBOMFields = SchedulableFields
                .Select(x => new BOMFieldDefintion_Revit(Doc, x))
                .ToList<BOMFieldDefinition>();

            DefaultFieldList = getDefaultBomFieldList();
        }

        private ObservableCollection<BOMFieldDefinition> getDefaultBomFieldList()
        {
            var defaultFields = new [] {
                Get("Mark", "Tag", 0.5/12.0),
                Get("Count","Qty",0.5/12.0),
                Get("Size","Size",1/12.0),
                Get("Product Short Description","Description",3/12.0),
                Get("Part Material","Material",3/12.0),
                Get("Length","Length",1.5/12.0)
            }.Where(x => x != null);

            return new ObservableCollection<BOMFieldDefinition>(defaultFields);
        }

        public BOMFieldDefintion_Revit Get(string fieldName, string header, double width)
        {
            var field = ModelBOMFields
                .OfType<BOMFieldDefintion_Revit>()
                .FirstOrDefault(x => x.ParameterName == fieldName);

            if (field == null) return null;

            field.ColumnHeader = header;
            field.ColumnWidth = width;

            return field;
        }

        /// <summary>
        /// Creates a temporary Bill of Materials (i.e. single category sched) so that we can get 
        /// a list of schedulable fields particular to that schedule. 
        /// </summary>
        private List<SchedulableField> getBomSchedulableFields()
        {
            List<SchedulableField> _return = null;

            var categoryId = new ElementId(
                Assembly
                .GetMemberIds()
                .GroupBy(x => Doc.GetElement(x).Category.Id.IntegerValue)
                .OrderBy(x => x.Count())
                .First()
                .Key);

            using (Transaction t = new Transaction(Doc, "Assembly Manager: Create Bill of Materials"))
            {
                t.Start();
                
                ViewSchedule billOfMaterials = AssemblyViewUtils
                    .CreateSingleCategorySchedule(Doc, Assembly.Id, categoryId);

                _return = billOfMaterials.Definition.GetSchedulableFields()
                    .Select(x => x)
                    .Where(x => x.ParameterId.IntegerValue < 0) //handles shared and projectParams (typically not shown in UI)
                    .ToList();

                t.RollBack();
            }

            return _return;
        }
    }

}
