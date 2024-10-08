

namespace PlayingWithMEP
{
    using Autodesk.Revit.UI;
    using System.Reflection;
    using System.Windows.Media.Imaging;
    using Autodesk.Revit.DB.Events;
    using System;
    using Autodesk.Revit.ApplicationServices;
    using ricaun.Revit.UI.Tasks;
    
    
    public class App : IExternalApplication
    {
        private static RevitTaskService revitTaskService;

        public static IRevitTask RevitTask => revitTaskService;

        public Result OnStartup (UIControlledApplication application)
        {
            revitTaskService = new RevitTaskService(application);
            revitTaskService.Initialize();

            SetupInterface ui = new SetupInterface ();
            
            ui.Initialize (application);


            return Result.Succeeded;
        }
 

        public Result OnShutdown(UIControlledApplication application)
        {

            return Result.Succeeded;
        }
    }
}
