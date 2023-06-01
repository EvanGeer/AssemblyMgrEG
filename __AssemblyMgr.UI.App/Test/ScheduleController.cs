using AssemblyMgrShared.DataModel;
using System.Collections.Generic;

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
