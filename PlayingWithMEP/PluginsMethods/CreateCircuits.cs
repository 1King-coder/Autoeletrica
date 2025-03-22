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
    internal class CreateCircuits : IExternalCommand
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

            List<FamilyInstance> panels  = utils.GetAllElectricalEquipments();

            List<WireType> wireTypes = utils.GetAllWireType();

            IList<Reference> preSelectedEls = sel.PickObjects(ObjectType.Element, new SelectionFilterDispositives());

            if (preSelectedEls.Count == 0)
            {
                return Result.Failed;
            }

            List<ElementId> selectedEls = utils.Map<Reference, ElementId>(preSelectedEls, (Reference x) => x.ElementId).ToList();

            CriacaoCircuitosForm createCircsForm = new CriacaoCircuitosForm(App.RevitTask, doc, wireTypes, panels, selectedEls);

            createCircsForm.Show();

            return Result.Succeeded;
        }
    }
}
