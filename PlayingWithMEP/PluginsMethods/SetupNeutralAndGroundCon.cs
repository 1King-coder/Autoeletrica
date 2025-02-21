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
    internal class SetupNeutralAndGroundCon : IExternalCommand
    {
        private UIApplication uiapp;
        private Document doc;
        private Selection sel;


        private Utils utils;
        private Automations.IdentifyCircuitsClass identify;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;

            Utils utils = new Utils(doc);

            FamilyInstance el = utils.pickElement(sel, new SelectionFilterPanels());

            ElectricalClasses.Panel panel = new ElectricalClasses.Panel(el, doc);


            try
            {

                (new ProjectAutomations.GeneralShortAutomations(doc)).SetupCircuitsConnections(panel);
                TaskDialog.Show("Sucesso", "circuitos configurados!");
            }
            catch (Exception e)
            {
                TaskDialog.Show("Erro", e.Message);
            }

            return Result.Succeeded;
        }
    }
}
