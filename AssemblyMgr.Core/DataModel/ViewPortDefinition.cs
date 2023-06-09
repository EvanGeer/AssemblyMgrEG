using AssemblyMgr.Core.Geometry;
using System;
using System.Xml.Serialization;

namespace AssemblyMgr.Core.DataModel
{
    [XmlInclude(typeof(ViewPortModel))]
    [XmlInclude(typeof(ViewPortSchedule))]
    //[XmlInclude(typeof(ViewPortCustomBom))]
    public class ViewPortDefinition
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Direction3d Direction { get; set; }
        public Box2d Outline { get; set; }
        public ViewPortType Type { get; set; }
        public string Title { get; set; }
        public bool IgnoreWelds { get; set; }
        public string ViewTemplate { get; set; }
    }
}
