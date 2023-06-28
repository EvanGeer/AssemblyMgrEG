namespace AssemblyMgr.Core.DataModel
{
    public class ViewPortDefinition_Model : ViewPortDefinition
    {
        public int Scale { get; set; }

        public string JointTag { get; set; }
        public string PipeTag { get; set; }
        public string FittingTag { get; set; }
        public bool HasTags { get; set; }
        public bool HasDimensions { get; set; }
        public ItemType ItemsToTag { get; set; }

        public string GetTagName(ItemType type)
        {
            switch (type)
            {
                case ItemType.Pipe: return PipeTag;
                case ItemType.Fitting: return FittingTag;
                case ItemType.Joint: return JointTag;
                default: return null;
            }
        }
    }
}
