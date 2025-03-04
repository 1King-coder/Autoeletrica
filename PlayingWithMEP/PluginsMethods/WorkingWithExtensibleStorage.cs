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
    internal class WorkingWithExtensibleStorage : IExternalCommand
    {
        private UIApplication uiapp;
        private Document doc;
        private Selection sel;


        private Utils utils;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            this.uiapp = commandData.Application;
            this.doc = this.uiapp.ActiveUIDocument.Document;


            return Result.Succeeded;
        }
    }
}
