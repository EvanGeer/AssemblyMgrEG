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
        [XmlArrayItem(Type = typeof(ViewPortDefinition_Model))]
        [XmlArrayItem(Type = typeof(ViewPortDefinition_Schedule))]
        //[XmlArrayItem(Type = typeof(ViewPortCustomBom))]
        public ObservableCollection<ViewPortDefinition> ViewPorts { get; set; }
            = new ObservableCollection<ViewPortDefinition>();
        public int TitleBlockId { get; set; }
        public string TitleBlock { get; set; }
    }
}
