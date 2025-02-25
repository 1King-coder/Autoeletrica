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
using ECs = AutoEletrica.ElectricalClasses;


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

            Transaction trans = new Transaction(doc);

            trans.Start("Selection");

            FamilyInstance el = utils.pickElement(sel);
            
            trans.Commit();

            ECs.Panel panel = new ECs.Panel(el, doc);
            

            return Result.Succeeded;
        }
    }
}
