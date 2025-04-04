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
using System.Windows.Forms;
using System.Windows;
using ricaun.Revit.UI.Tasks;
using ricaun.Revit.Mvvm;
using AutoEletrica.Sources;
using System.Windows.Controls;
using Automations = AutoEletrica.ProjectAutomations;



namespace AutoEletrica
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DiagramaTrifilar : IExternalCommand
    { 

        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {

            new GenerateThreeLineDiagramForm(App.RevitTask).Show();

            return Result.Succeeded;
        }

    
    }
}
