using System.ComponentModel;

namespace AssemblyMgr.Core.DataModel
{
    public enum ViewPortType
    {
        [Description("< Select Type... >")]
        None = 0,
        [Description("Plan")]
        ModelPlan,
        [Description("Elevation")]
        ModelElevation,
        [Description("Ortho")]
        ModelOrtho,
        [Description("Schedule")]
        Schedule,
    }
}
