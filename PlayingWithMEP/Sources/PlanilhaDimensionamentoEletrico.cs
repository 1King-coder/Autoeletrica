using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlayingWithMEP
{
    internal class PlanilhaDimensionamentoEletrico : GoogleSheetsManager
    {
        public PlanilhaDimensionamentoEletrico(string spreadsheetId) : base(spreadsheetId) { }

        public void SendCircuitsDataToSheets(ElectricalClasses.Panel panel)
        {
            
            // TODO: Implementar a escrita dos dados de circuitos na planilha
            // B8:C8;E8:F8;N8:R8;
            string getCircuitLoadBoardRange (int rowNum)
            {
                return $"B{rowNum}:C{rowNum};E{rowNum}:F{rowNum};N{rowNum}:R{rowNum}";
            }

            string getCircuitCalcRange (int rowNum)
            {
                return $"N{rowNum}";
            }

            string sheet1 = "Quadro de Carga";
            string sheet2 = "Dimensionamento das Seções";
            int row = 8;

            foreach (ElectricalClasses.Circuit c in panel.AssignedCircuits)
            {
                List<object> circuitData_1 = new List<object>() { 
                    c.circuitNumber,
                    c.Name,
                };


                List<object> circuitData_2 = new List<object>() {
                    c.voltage,
                    c.Name.Contains("Iluminação") ? "F + N" : c.scheme,
                };
                List<object> circuitData_3 = new List<object>() {
                    c.numOfDispositivesByLoad["Lamps"]["100"],
                    c.numOfDispositivesByLoad["Lamps"]["60"],
                    c.numOfDispositivesByLoad["TUGs-TUEs"]["100"],
                    c.numOfDispositivesByLoad["TUGs-TUEs"]["600"],
                    c.numOfDispositivesByLoad["TUGs-TUEs"]["TUE"]
                };

                List<object> circuitData_4 = new List<object>() { c.length };

                string[] ranges = getCircuitLoadBoardRange(row).Split(';');

                this.editData(sheet1, ranges[0], circuitData_1);
                this.editData(sheet1, ranges[1], circuitData_2);
                this.editData(sheet1, ranges[2], circuitData_3);
                this.editData(sheet2, getCircuitCalcRange(row), circuitData_4);
                Thread.Sleep(250);

                row++;
                
            }
        }

        private Dictionary<string, string> GetAllCircuitsDataFromCalcColumn(ElectricalClasses.Panel panel, string column)
        {
            int numOfCircuits = panel.AssignedCircuits.Count;
            string rowsToGetData = $"{column}9:{column}{9 + numOfCircuits}";
            string rowsToGetCircuitsNum = $"B9:B{9 + numOfCircuits}";

            Dictionary<string, string> circuitData = new Dictionary<string, string>();
            IList<IList<object>> data = this.readData("Dimensionamento das Seções", rowsToGetData);
            IList<IList<object>> CircuitsInSpreadsheetOrder = this.readData("Dimensionamento das Seções", rowsToGetCircuitsNum);

            for (int i = 0; i < numOfCircuits; i++)
            {
                if (data[i][0] != null)
                {
                    circuitData.Add(CircuitsInSpreadsheetOrder[i][0] as string, data[i][0] as string);
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
