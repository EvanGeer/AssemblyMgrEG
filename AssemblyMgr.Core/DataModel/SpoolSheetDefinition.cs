using AssemblyMgr.Core.Serialization;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace AssemblyMgr.Core.DataModel
{
    public class SpoolSheetDefinition : ISettings
    {
        public SpoolSheetDefinition()
        {
        }

        [XmlArray()]
        [XmlArrayItem(Type = typeof(ViewPortModel))]
        [XmlArrayItem(Type = typeof(ViewPortSchedule))]
        //[XmlArrayItem(Type = typeof(ViewPortCustomBom))]
        public ObservableCollection<ViewPortDefinition> ViewPorts { get; set; }
            = new ObservableCollection<ViewPortDefinition>();
        public int TitleBlockId { get; set; }
        public string TitleBlock { get; set; }
        //public string AssemblyName { get; set; }

        //public SpoolSheetDefinition(string assemblyName)
        //{
        //    AssemblyName = assemblyName;
        //}
    }
}
