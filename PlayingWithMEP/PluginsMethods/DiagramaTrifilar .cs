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
using System.Windows.Forms;
using System.Windows;
using ricaun.Revit.UI.Tasks;
using ricaun.Revit.Mvvm;
using PlayingWithMEP.Sources;
using System.Windows.Controls;
using Automations = PlayingWithMEP.ProjectAutomations;



namespace PlayingWithMEP
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DiagramaTrifilar : IExternalCommand
    { private UIApplication uiapp;
        private Document doc;
        private Selection sel;


        private Utils utils;
        private Automations.GenerateDiagramsClass genDiag;
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;
            this.uiapp = commandData.Application;
            this.doc = uiapp.ActiveUIDocument.Document;
            this.sel = uiapp.ActiveUIDocument.Selection;

            new GenerateThreeLineDiagramForm(App.RevitTask).Show();

            return Result.Succeeded;
        }

    
    }
}
