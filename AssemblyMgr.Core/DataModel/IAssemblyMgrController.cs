using AssemblyMgr.Core.DataModel;
using System.Collections.Generic;

public interface IAssemblyMgrController
{
    List<BOMFieldDefinition> BOMFields { get; }
    List<string> TitleBlocks { get; }
    List<string> ViewTemplates { get; }
    List<string> ScheduleTemplates { get; }
    string GetTitleBlockImage(string titleBlock);
    string RefreshImage(string titleBlock);
}
