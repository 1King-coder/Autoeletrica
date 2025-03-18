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

namespace AutoEletrica
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class ChangeDispositiveTags : IExternalCommand
    {
        private UIApplication uiapp;
        private Document doc;


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            doc = commandData.Application.ActiveUIDocument.Document;
            uiapp = commandData.Application;

            var autos = new Automations.GeneralShortAutomations(doc);

            autos.changeDispositiveTagFor100Load(uiapp);


            return Result.Succeeded;
        }
    }
}
