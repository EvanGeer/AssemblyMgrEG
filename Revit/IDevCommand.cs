using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyMgrRevit.Revit
{
    /// <summary>Interface for custom Revit Dev Environment</summary>
    public interface IDevCommand
    {
        Result ExecuteDev(UIApplication uiApp);

        string ProductPrefix { get; }
        string Product { get; }
        string SubGroup { get; }
        string Name { get; }

        string ModelName { get; }
        string ModelYear { get; }
    }
}
