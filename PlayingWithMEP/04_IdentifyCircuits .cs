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


namespace PlayingWithMEP
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class _04_IdentifyCircuits : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;

            

            Utils utils = new Utils(doc);
            GeometryUtils gUtils = new GeometryUtils(doc);
            Automations.IdentifyCircuitsMethod identify = new Automations.IdentifyCircuitsMethod(doc);

            Transaction trans = new Transaction(doc);

            trans.Start("Selection");

            Reference el = utils.pickElementRef(sel);
            
            trans.Commit();

            //ECs.Panel panel = new ECs.Panel(doc.GetElement(el) as FamilyInstance, doc);

            identify.IdentifyDispositiveCircuit(new ECs.Dispositive(doc.GetElement(el), doc), el);

            //identify.identifyAllCircuitsFromPanel(panel);
            
            return Result.Succeeded;
        }
    }
}
