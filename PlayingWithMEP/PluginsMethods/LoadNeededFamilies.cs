using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AutoEletrica.Sources;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Electrical;
using ECs = AutoEletrica.ElectricalClasses;
using Automations = AutoEletrica.ProjectAutomations;
using System.Windows.Forms;
using System.IO;

namespace AutoEletrica
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class LoadNeededFamilies : IExternalCommand
    {
        private UIApplication uiapp;
        private Document doc;
        private Selection sel;


        private Utils utils;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            doc = commandData.Application.ActiveUIDocument.Document;
            uiapp = commandData.Application;

            sel = uiapp.ActiveUIDocument.Selection;
            
            try
            {
                FolderBrowserDialog fDialog = new FolderBrowserDialog();
                DialogResult fDialogResult = fDialog.ShowDialog();

                if (fDialog.SelectedPath == null || fDialogResult == DialogResult.Cancel || fDialogResult == DialogResult.Abort)
                {
                    return Result.Cancelled;
                }

                string path = fDialog.SelectedPath;

                Directory.GetFiles(path).ToList().ForEach((string filename) =>
                {
                    using (Transaction t = new Transaction(doc, "Load Family"))
                    {
                        if (Path.GetExtension(filename) == ".rfa")
                        {
                            t.Start();
                            doc.LoadFamily(filename);
                            t.Commit();
                        }
                    }
                }) ;
            } catch (Exception e)
            {
                TaskDialog.Show("Error", e.Message);
            }




            return Result.Succeeded;
        }
    }
}
