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
using ECs = AutoEletrica.ElectricalClasses;
using System.Windows.Forms;
using System.Windows;
using ricaun.Revit.UI.Tasks;
using ricaun.Revit.Mvvm;



namespace AutoEletrica
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SendingCircuitsDataToSheets : IExternalCommand
    { 
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;
            

            new SendCircuitsToSheets(App.RevitTask).Show();

            

            return Result.Succeeded;
        }

    
    }
}
