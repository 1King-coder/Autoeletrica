using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Electrical;
using ECs = PlayingWithMEP.ElectricalClasses;
using Automations = PlayingWithMEP.ProjectAutomations;
using System.Security.Principal;


namespace PlayingWithMEP
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DiagramaUnifilar : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;
            Diagrams diagrams = new Diagrams(doc);

            new GenerateSingleLineDiagramForm(App.RevitTask).Show();




            return Result.Succeeded;
        }
    }
}
