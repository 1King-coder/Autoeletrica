using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AutoEletrica.Sources;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Electrical;
using ECs = AutoEletrica.ElectricalClasses;
using Automations = AutoEletrica.ProjectAutomations;

namespace AutoEletrica.PluginsMethods
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class Debugger : IExternalCommand
    {
        private UIApplication uiapp;
        private Document doc;
        private Selection sel;


        private Utils utils;
        private Automations.IdentifyCircuitsClass identify;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            PlanilhaDimensionamentoEletrico p = new PlanilhaDimensionamentoEletrico("1wVtyuVu6z8GwxDdQ2vmJiUagvQQ_OKLVDvF5IUzW2bI");

            TaskDialog.Show("Planilha", $"{Convert.ToInt32( p.readData("Quadro de Carga", "X7:Z7").Last()[2])}");

            return Result.Succeeded;
        }
    }
}
