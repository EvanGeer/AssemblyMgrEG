using System;
using AssemblyMgr.Core.DataModel;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AssemblyMgr.Revit.Core
{
    public abstract class ExternalCommandBase : IExternalCommand
    {
        public UIApplication UiApp { get; private set; }
        public UIDocument UiDoc => UiApp.ActiveUIDocument;
        public Document Doc => UiApp.ActiveUIDocument.Document;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UiApp = commandData.Application;

            return UseTransactionGroup
                ? WrapInTransactionGroup(Execute)
                : Execute();
        }


        private Result WrapInTransactionGroup(Func<Result> action)
        {
            using (var tGroup = new TransactionGroup(Doc, CommandName))
            {
                tGroup.Start();
                var result = action.Invoke();

                if (result == Result.Succeeded)
                    tGroup.Assimilate();

                else
                    tGroup.RollBack();

                return result;
            }
        }

        public virtual bool UseTransactionGroup => true;
        public virtual string CommandName => GetType().Name;
        public virtual FileInfo SettingsFile => new FileInfo(Path.Combine(
            Constants.DataFolder.FullName,
            $"{CommandName}.xml"));
        public abstract Result Execute();
    }
}
