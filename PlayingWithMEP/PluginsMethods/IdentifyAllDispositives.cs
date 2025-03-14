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
using Automations = AutoEletrica.ProjectAutomations;
using AutoEletrica.Sources;



namespace AutoEletrica
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class IdentifyDispositives : IExternalCommand
    {
        private UIApplication uiapp;
        private Document doc;
        private Selection sel1;
        private Selection sel2;


        private Utils utils;
        private Automations.IdentifyCircuitsClass identify;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet) 
        { 

            this.uiapp = commandData.Application;
            this.doc = uiapp.ActiveUIDocument.Document;
            this.sel1 = uiapp.ActiveUIDocument.Selection;
            this.sel2 = uiapp.ActiveUIDocument.Selection;


            this.utils = new Utils(doc);
            this.identify = new Automations.IdentifyCircuitsClass(doc);
        



            List<FamilyInstance> els = utils.pickElements(sel1, new SelectionFilterDispositives());
            XYZ leaderEndPt = null;
            XYZ leaderElbowPt = null;
            try
            {
                if (els.Count == 1)
                {
                    FamilyInstance el = els.Last();
                    if (utils.isElectricEquipment(els.Last()))
                    {
                        ECs.Panel panel = new ECs.Panel(el, doc);
                        identify.indentifyAllDispositivesFromPanel(panel);
                        return Result.Succeeded;
                    }
                }
                leaderEndPt = sel1.PickPoint("Selecione a posição da ponta da linha de chamada");
                leaderElbowPt = sel2.PickPoint("Selecione a posição do cotovelo da linha de chamada");

                identifyThings(els, leaderEndPt, leaderElbowPt);

            } catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
            {
                
                identifyThings(els, null, null); 


            }

            return Result.Succeeded;
        }

        private void identifyThings(List<FamilyInstance> els, XYZ leaderEndPt, XYZ leaderElbowPt)
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


                    identify.identifyMultipleDispositiveCircuitScheme(sortedCircuits, leaderEndPt, leaderElbowPt);
                    return;
                }

                FamilyInstance el = els.Last();

                if (utils.isElectricEquipment(el))
                {
                    identify.indentifyAllDispositivesFromPanel(new ECs.Panel(el, doc));
                    return;
                }

                identify.identifyDispositiveCircuitScheme(new ECs.Dispositive(el as Element, doc), leaderEndPt, leaderElbowPt);
                return;
                

                
            }
        }

    }
}
