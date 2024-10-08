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
using System.Security.Principal;


namespace PlayingWithMEP
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DiagramaUnifilar : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;
            Diagrams diagrams = new Diagrams(doc);

            PlanilhaDimensionamentoEletrico planilha = new PlanilhaDimensionamentoEletrico("1MZt_KFsS692brVrzg6c-06q4siw1l7bPfPLvwIXfC_c");

            Automations.GenerateDiagramsClass diagGen = new Automations.GenerateDiagramsClass(doc, planilha);

            Utils utils = new Utils(doc);

            Transaction trans = new Transaction(doc);

            trans.Start("Selection");

            Reference el = utils.pickElementRef(sel);
            trans.Commit();
            ECs.Panel panel = new ECs.Panel(doc.GetElement(el) as FamilyInstance, doc);

            diagGen.GenSingleLineDiagramFromPanel(panel);

            

            return Result.Succeeded;
        }
    }
}
