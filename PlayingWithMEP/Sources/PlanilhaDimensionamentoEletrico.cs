using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Architecture;
using System.Data.SqlClient;

namespace AutoEletrica
{
    internal class PlanilhaDimensionamentoEletrico : GoogleSheetsManager
    {
        public PlanilhaDimensionamentoEletrico(string spreadsheetId) : base(spreadsheetId) { }

        public void SendCircuitsDataToSheets(ElectricalClasses.Panel panel)
        {

            string sheet1 = "Quadro de Carga";
            string sheet2 = "Dimensionamento das Seções";

            

            List<IList<object>> circuitData_1 = this.FormatCircuitsDataToSend(panel.AssignedCircuits);

            List<IList<object>> circsLengths = new List<IList<object>>();

            panel.AssignedCircuits.ForEach ((ElectricalClasses.Circuit c) => circsLengths.Add(new List<object>() { c.length}));

            string range1 = $"B8:Z{panel.AssignedCircuits.Count() + 8}";
            string range2 = $"O9:O{panel.AssignedCircuits.Count() + 9}";
            try
            {

                this.editData(sheet1, range1, circuitData_1);
                this.editData(sheet2, range2, circsLengths);
                
                
            } catch (Exception e)
            {
                TaskDialog.Show("Error", e.ToString());
            }

            
        }

        public List<IList<object>> FormatCircuitsDataToSend(List<ElectricalClasses.Circuit> circuits)
        {
            List<IList<object>> formattedData = new List<IList<object>>();

            foreach (ElectricalClasses.Circuit c in circuits)
            {
                IList<object> circuitData_1 = new List<object>()
                {
                    c.circuitNumber,
                    c.Name,
                    null,
                    c.voltage,
                    c.Name.Contains("Iluminação") ? "F + N" : c.scheme,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    c.numOfDispositivesByLoad["Lamps"]["60"],
                    c.numOfDispositivesByLoad["Lamps"]["100"],
                    c.numOfDispositivesByLoad["TUGs-TUEs"]["100"],
                    c.numOfDispositivesByLoad["TUGs-TUEs"]["600"],
                    c.numOfDispositivesByLoad["TUGs-TUEs"]["TUE"],
                    null,
                    null,
                    null,
                    null,
                    null,
                    c.phaseALoad,
                    c.phaseBLoad,
                    c.phaseCLoad
                };

                formattedData.Add(circuitData_1);
            }
            


            return formattedData;
        }

        //public void SendRoomsToSheets(Document doc)
        //{

        //    // TODO: Implementar a escrita dos dados de circuitos na planilha
        //    // B8:C8;E8:F8;N8:R8;
        //    string getRange(int rowNum)
        //    {
        //        return $"A{rowNum}:D{rowNum}";
        //    }
        //    Utils ut = new Utils(doc);

        //    List<Room> projectRooms = ut.getRoomsFromLevel(doc, doc.GetElement(doc.ActiveView.LevelId) as Level);

        //    string sheet = "Informações do projeto";

        //    int row = 7;

        //    foreach (Room r in projectRooms)
        //    {
        //        List<object> roomData = new List<object>() {
        //            r.Name,
        //            "",
        //            Math.Round(r.Area*Math.Pow(0.3048, 2), 2),
        //            Math.Round(r.Perimeter*0.3048, 2),
        //        };


        //        this.editData(sheet, getRange(row), roomData);
                
        //        Thread.Sleep(150);

        //        row++;

        //    }
        //}

        private Dictionary<string, string> GetAllCircuitsDataFromCalcColumn(ElectricalClasses.Panel panel, string column)
        {
            int numOfCircuits = panel.AssignedCircuits.Count;
            string rowsToGetData = $"{column}9:{column}{9 + numOfCircuits}";
            string rowsToGetCircuitsNum = $"B9:B{9 + numOfCircuits}";

            Dictionary<string, string> circuitData = new Dictionary<string, string>();
            IList<IList<object>> data = this.readData("Dimensionamento das Seções", rowsToGetData);
            Thread.Sleep(150);
            IList<IList<object>> CircuitsInSpreadsheetOrder = this.readData("Dimensionamento das Seções", rowsToGetCircuitsNum);

            for (int i = 0; i < numOfCircuits; i++)
            {
                
                if (!panel.AssignedCircuits[i].Name.Contains("Reserva") && data[i][0] != null)
                {
                    circuitData.Add(CircuitsInSpreadsheetOrder[i][0] as string, data[i][0] as string);
                } else
                {
                    circuitData.Add(CircuitsInSpreadsheetOrder[i][0] as string, "0");

                }
            }

            return circuitData;
        }

        public Dictionary<string, string> GetAllCircuitsCableSeccion (ElectricalClasses.Panel panel)
        {

            return GetAllCircuitsDataFromCalcColumn(panel, "R");
        }

        public Dictionary<string, string> GetAllCircuitsBreakersAmps(ElectricalClasses.Panel panel)
        {
            return GetAllCircuitsDataFromCalcColumn(panel, "V");
        }

        public Dictionary<string, string> GetAllCircuitsTemperatureFactors(ElectricalClasses.Panel panel)
        {
            return GetAllCircuitsDataFromCalcColumn(panel, "J");
        }

        public Dictionary<string, string> GetAllCircuitsGroupFactors(ElectricalClasses.Panel panel)
        {
            return GetAllCircuitsDataFromCalcColumn(panel, "I");
        }

        public Dictionary<string, string> GetAllCircuitsVoltageDrops(ElectricalClasses.Panel panel)
        {
            return GetAllCircuitsDataFromCalcColumn(panel, "P");
        }

        public string GetDemandedLoadFromPanel ()
        {
            string demandedLoad = this.readData("Cálculo de Demanda", "G17:G17").Last().Last() as string;

            return demandedLoad;
        }

        public Dictionary<string, Dictionary<string, string>> GetCircuitsLoadPerPhase (ElectricalClasses.Panel panel)
        {
            int numOfCircuits = panel.AssignedCircuits.Count;
            string rowsToGetPhasesLoadNum = $"X8:Z{8 + numOfCircuits}";
            string rowsToGetCircuitsNum = $"B8:B{8 + numOfCircuits}";
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();


            IList<IList<object>> phasesLoad = this.readData("Quadro de Carga", rowsToGetPhasesLoadNum);
            IList<IList<object>> circuitsNum = this.readData("Quadro de Carga", rowsToGetCircuitsNum);

            for (int i = 0; i < numOfCircuits; i++)
            {
                string circNum = circuitsNum[i].Last() as string;
                result.Add(circNum, new Dictionary<string, string>());
                result[circNum].Add("A", phasesLoad[i][0] as string); 
                result[circNum].Add("B", phasesLoad[i][1] as string);
                result[circNum].Add("C", phasesLoad[i][2] as string);
            }

            return result;

        }


    }
}
