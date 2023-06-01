using AssemblyMgr.Core.DataModel;
using System.Collections.Generic;

public interface IAssemblyMgrController
{
    //IScheduleController ScheduleController { get; }
    //ITitleBlockController TitleBlockController { get; }
    List<BOMFieldDefinition> GetBOMFields();
    // ToDo: should this be Dictionary<int,string> ?
    List<string> GetTitleBlocks();  
    string GetTitleBlockImage(string name);
    //ToDo: allow users to re-generate the titleblock images
    //void ClearImageCache()

}

//public interface IScheduleController
//{
//    List<BOMFieldDefinition> GetBOMFields();

//}


//public class TitleBlock
//{
//    public string ImagePath { get; set; }
//    public string Name { get; set; }
//}

//public interface ITitleBlockController
//{
//    List<string> GetTitleBlocks();
//    string GetTitleBlockImage(string name);
//    //ToDo: allow users to re-generate the titleblock images
//    //void ClearImageCache()
//}