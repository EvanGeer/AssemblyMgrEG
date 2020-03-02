using System.Collections.ObjectModel;

namespace AssemblyMgrEG.Revit
{
    public class BomFieldCollection : ObservableCollection<AssemblyMgrBomField>
    {

    }
    public class AssemblyMgrBomField
    {
        public string parameterName { get; set; }
        public string columnHeader { get; set; }
        public double columnWidth { get; set; }

        public AssemblyMgrBomField(string ParameterName, string Header, double Width)
        {
            parameterName = ParameterName;
            columnHeader = Header;
            columnWidth = Width;
        }
    }

}
