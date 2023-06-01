namespace AssemblyManagerUI.Test
{
    internal class AssemblyMgrController : IAssemblyMgrController
    {
        public IScheduleController ScheduleController { get; } = new ScheduleController();
        public ITitleBlockController TitleBlockController { get; } = new TitleBlockController();
    }
}
