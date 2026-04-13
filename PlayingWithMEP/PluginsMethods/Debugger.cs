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
using System.Windows;

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
            doc = commandData.Application.ActiveUIDocument.Document;
            uiapp = commandData.Application;

            sel = uiapp.ActiveUIDocument.Selection;
            Utils utils = new Utils(doc);

            FamilyInstance disp = utils.pickElement(sel, new SelectionFilterDispositives());

            List<List<Conduit>> conduitRunsConnected = Utils.GetAllConduitRunsConnectedToFamilyInstance(disp, doc);

            MessageBox.Show("Conduit Runs Connected: " + conduitRunsConnected.Count);

            return Result.Succeeded;
        }
    }
}
