namespace AssemblyMgr.Core.DataModel
{
    public interface IStringSearchable
    {
        bool PassesSearch(string queryString);
    }
}