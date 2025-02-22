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
using AutoEletrica.Sources;

namespace AutoEletrica
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class AssociaComandosAInterruptores: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;

            Utils utils = new Utils(doc);



            FamilyInstance el = utils.pickElement(sel, new SelectionFilterPanels());

            ElectricalEquipment panel = el.MEPModel as ElectricalEquipment;
            String abcd = "abcdefghijklmnopqrstuvwxyz";

            int counter = 0;
            foreach (ElectricalSystem circ in panel.GetAssignedElectricalSystems())
            {


                foreach (FamilyInstance disp in circ.Elements) {
                    
                    
                    if (disp.Category.Name == "Dispositivos de iluminação")
                    {
                        Transaction trans = new Transaction(doc);

                        trans.Start("Changing commands Ids");
                        disp.LookupParameter("ID do comando").Set($"{el.LookupParameter("Número do QD").AsValueString()}{abcd[counter]}");
                        trans.Commit();
                        counter++;
                    }
                    
                };
                
            };

            

            

            return Result.Succeeded;
        }
    }
}
