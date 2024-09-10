﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Electrical;

namespace PlayingWithMEP
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class _01_TestToGetEquipment: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;

            Utils utils = new Utils(doc);

            Transaction trans = new Transaction(doc);

            trans.Start("Selection");

            FamilyInstance el = utils.pickElement(sel);
            
            ElectricalEquipment panel = el.MEPModel as ElectricalEquipment;

            ISet<ElectricalSystem> electricalSystemsSet = panel.GetAssignedElectricalSystems();

            List<ElectricalSystem> electricalSystemsList = utils.ESSetToList(electricalSystemsSet);

            

            

            string parameters = electricalSystemsList[0].get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_LOAD).AsValueString();



            trans.Commit();

            return Result.Succeeded;
        }
    }
}
