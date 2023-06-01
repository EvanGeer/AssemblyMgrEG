namespace AssemblyMgrShared.DataModel
{
    public interface IStringSearchable
    {
        bool PassesSearch(string queryString);
    }
}