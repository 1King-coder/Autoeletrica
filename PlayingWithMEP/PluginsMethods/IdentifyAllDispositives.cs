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
using Automations = PlayingWithMEP.ProjectAutomations;
using PlayingWithMEP.Sources;
using System.Security.Principal;
using System.Windows.Controls;
using static PlayingWithMEP.ElectricalClasses;


namespace PlayingWithMEP
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class IdentifyDispositives : IExternalCommand
    {
        private UIApplication uiapp;
        private Document doc;
        private Selection sel;


        private Utils utils;
        private Automations.IdentifyCircuitsClass identify;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet) 
        { 

            this.uiapp = commandData.Application;
            this.doc = uiapp.ActiveUIDocument.Document;
            this.sel = uiapp.ActiveUIDocument.Selection;


            this.utils = new Utils(doc);
            this.identify = new Automations.IdentifyCircuitsClass(doc);
        



            List<FamilyInstance> els = utils.pickElements(sel, new SelectionFilterDispositives());
            
            try
            {
                XYZ pickedPt = sel.PickPoint();
                pickedPt.Add(new XYZ(0, 0.135 / 0.3048, 0));
                identifyThings(els, pickedPt);

            } catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
            {
                identifyThings(els, null);

            }

            return Result.Succeeded;
        }

        private void identifyThings(List<FamilyInstance> els, XYZ placementPt)
        {
            if (els != null)
            {
                if (els.Count > 1)
                {
                    List<ECs.Dispositive> selectedDispositives = els.ConvertAll(e => new ECs.Dispositive(e as Element, doc));

                    List<ECs.Dispositive> sortedCircuits = new List<ECs.Dispositive>(selectedDispositives.OrderBy((c) =>
                    {
                        if (c.EScircuit.CircuitNumber.Split(',').ToList().Count() > 1) 
                        {
                            return Convert.ToInt32(c.EScircuit.CircuitNumber.Split(',')[0]); 
                        }
                        else
                        {
                            return Convert.ToInt32(c.EScircuit.CircuitNumber);
                        }
                    }));

                    identify.identifyMultipleDispositiveCircuitScheme(sortedCircuits, placementPt);
                    return;
                }

                FamilyInstance el = els.Last();

                if (utils.isDispositive(el as Element) || utils.isLuminary(el as Element))
                {
                    identify.identifyDispositiveCircuitScheme(new ECs.Dispositive(el as Element, doc), placementPt);
                    return;
                }

                if (utils.isElectricEquipment(el))
                {
                    identify.indentifyAllDispositivesFromPanel(new ECs.Panel(el, doc));
                }
            }
        }

    }
}
