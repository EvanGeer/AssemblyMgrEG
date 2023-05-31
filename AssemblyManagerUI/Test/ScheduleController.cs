using AssemblyMgrShared.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyManagerUI.Test
{
    internal class ScheduleController : IScheduleController
    {
        public List<BOMFieldDefinition> GetBOMFields() =>
            new List<BOMFieldDefinition>
            {
                new BOMFieldDefinition("Tag"),
                new BOMFieldDefinition("Description"),
                new BOMFieldDefinition("Length"),
                new BOMFieldDefinition("End Prep 1"),
                new BOMFieldDefinition("End Prep 2"),
            };
    }
}
