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
using Autodesk.Revit.DB.Architecture;


namespace AutoEletrica
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SendRoomsDataToSheets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {


            new SendRoomsToSheets(App.RevitTask).Show();

            return Result.Succeeded;
        }
    }
}
