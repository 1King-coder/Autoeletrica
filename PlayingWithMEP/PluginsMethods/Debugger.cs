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
            doc = commandData.Application.ActiveUIDocument.Document;
            uiapp = commandData.Application;

            sel = uiapp.ActiveUIDocument.Selection;
            var autos = new Automations.CreateCircuits(doc);
            
            utils = new Utils(doc);

            IList<Element> els = sel.PickElementsByRectangle(new SelectionFilterDispositives());

            IList<ElementId> elsIds = utils.Map<Element, ElementId>(els, (Element el) => el.Id);

            autos.CreateCircuit("circ Teste", elsIds);




            return Result.Succeeded;
        }
    }
}
