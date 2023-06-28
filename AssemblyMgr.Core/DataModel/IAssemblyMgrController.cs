using System.Collections.Generic;

public interface IAssemblyMgrController
{
    List<string> TitleBlocks { get; }
    List<string> ViewTemplates { get; }
    List<string> ScheduleTemplates { get; }
    List<string> TagTypes { get; }
    string GetTitleBlockImage(string titleBlock);
    string RefreshImage(string titleBlock);
}
