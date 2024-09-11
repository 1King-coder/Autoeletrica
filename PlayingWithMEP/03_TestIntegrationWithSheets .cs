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


namespace PlayingWithMEP
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class _03_TestIntegrationWithSheets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;

            GoogleSheetsManager sheetsApi = new GoogleSheetsManager("1MZt_KFsS692brVrzg6c-06q4siw1l7bPfPLvwIXfC_c");
            

            Utils utils = new Utils(doc);

            Transaction trans = new Transaction(doc);

            trans.Start("Selection");

            FamilyInstance el = utils.pickElement(sel);
            
            trans.Commit();

            ECs.Panel panel = new ECs.Panel(el, doc);

            List<object> data = new List<object>() { "Hello", "World", "1001" };

            sheetsApi.writeData("Quadro de Carga", "B8:D8", data);            

            return Result.Succeeded;
        }
    }
}
