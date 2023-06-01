using System.ComponentModel;

namespace AssemblyManagerUI.ViewModels
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
