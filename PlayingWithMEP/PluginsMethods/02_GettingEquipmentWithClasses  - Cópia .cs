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
using Autodesk.Revit.DB.Architecture;


namespace AutoEletrica
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class _02_GettingEquipmentWithClasses: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;
            

            Utils utils = new Utils(doc);

            utils.GetRevitLinks().ForEach((RevitLinkInstance rl)=>
            {
                Document linkedDoc = rl.GetLinkDocument();

                FilteredElementCollector fec = new FilteredElementCollector(linkedDoc);

                TaskDialog.Show("Revit Link", linkedDoc.Title);
                fec.OfClass(typeof(SpatialElement)).WhereElementIsNotElementType().ToElements().Cast<Room>().ToList().ForEach((Room e) =>
                {
                    TaskDialog.Show("Element", $"Name: {e.Name} \nArea: {utils.feetToMeters2(e.Area)} \nPerimeter: {utils.feetToMeters(e.Perimeter)}");
                });
            });

            return Result.Succeeded;
        }
    }
}
