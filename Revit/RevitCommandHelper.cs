using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;


namespace AssemblyMgrEG.Revit
{
    /// <summary>
    /// Class that aids in interactions with Revit.
    /// (wrote this with my team in my current role. Just makes life easier and code more readable)
    /// </summary>
    public class RevitCommandHelper
    {
        /// <summary>
        /// The CommandData passed from Revit.
        /// </summary>
        private ExternalCommandData _cmd;

        /// <summary>
        /// This field isn't used anywhere.
        /// </summary>
        public string _filepath;

        /// <summary>
        /// The name of the model that was opened when the command was called in Revit.
        /// </summary>
        public string modelName { get; set; }

        /// <summary>
        /// The name of the user called the command in Revit.
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// The project number of the model that was open when the command was called in Revit.
        /// </summary>
        public string projectNumber { get; set; }

        /// <summary>
        /// The package number of the model that was open when the command was called in Revit.
        /// </summary>
        public string packageNumber { get; set; }

        /// <summary>
        /// The version of revit currently being run
        /// </summary>
        public string revitVersion { get; set; }

        /// <summary>
        /// The file path of the model
        /// </summary>
        public string filePath { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cmd"></param>
        public RevitCommandHelper(ExternalCommandData cmd)
        {
            //Widen Scope
            _cmd = cmd;
            try
            {
                userName = App.Username;
                revitVersion = App.VersionNumber;
            }
            catch
            { }

            if (ActiveDoc.IsWorkshared)
            {
                try
                {
                    filePath = ActiveDoc.PathName;
                    projectNumber = ActiveDoc.ProjectInformation.Number;
                    packageNumber = ActiveDoc.ProjectInformation.LookupParameter("Package Number").AsString();
                }
                catch
                { }
                try
                {
                    modelName = ActiveDoc.Title.Substring(0, ActiveDoc.Title.IndexOf(App.Username) - 1);
                    modelName.Replace(".viewable", "");
                }
                catch
                {
                    modelName = ActiveDoc.Title;
                    modelName.Replace(".viewable", "");
                }
            }
            else
            {
                try
                {
                    projectNumber = ActiveDoc.ProjectInformation.Number;
                    modelName = ActiveDoc.Title;
                    filePath = ActiveDoc.PathName;
                    packageNumber = ActiveDoc.ProjectInformation.LookupParameter("Package Number").AsString();
                }
                catch
                {
                }
            }

            if (!modelName.EndsWith(".rvt"))
            {
                modelName += ".rvt";
            }
        }

        /// <summary>
        /// Getter and setter for CommandData.Application
        /// </summary>
        public UIApplication UiApp
        {
            get
            {
                try
                {
                    return _cmd.Application;
                }
                catch { }
                return null;
            }
        }

        /// <summary>
        /// Getter and setter for CommandData.Application.Application
        /// </summary>
        public Application App
        {
            get
            {
                try
                {
                    return _cmd.Application.Application;
                }
                catch { }
                return null;
            }
        }

        /// <summary>
        /// Getter and setter for CommandData.Application.ActiveUIDocument
        /// </summary>
        public UIDocument UiDoc
        {
            get
            {
                try
                {
                    return _cmd.Application.ActiveUIDocument;
                }
                catch { }
                return null;
            }
        }

        /// <summary>
        /// Getter  and setter for CommandData.Application.Document
        /// </summary>
        public Document ActiveDoc
        {
            get
            {
                try
                {
                    return UiDoc.Document;
                }
                catch { }
                return null;
            }
        }
    }

}

