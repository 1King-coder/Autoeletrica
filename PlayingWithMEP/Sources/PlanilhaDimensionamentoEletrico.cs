﻿using Autodesk.Revit.DB;
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
                    c.isNotReserveCircuit == 1 ? c.phaseALoad : "0",
                    c.isNotReserveCircuit == 1 ? c.phaseBLoad : "0",
                    c.isNotReserveCircuit == 1 ? c.phaseCLoad : "0"
                };

                formattedData.Add(circuitData_1);
            }
            


            return formattedData;
        }

        public void SendRoomsToSheets(List<Room> projectRooms)
        {

            // TODO: Implementar a escrita dos dados de circuitos na planilha
            // B8:C8;E8:F8;N8:R8;
            string getRange(int row1, int row2)
            {
                return $"A{row1}:D{row2}";
            }

            string sheet = "Informações do projeto";

            int row = 7;

            Utils ut = new Utils();

            List<IList<object>> roomsData = new List<IList<object>>();

            foreach (Room r in projectRooms)
            {
                IList<object> roomData = new List<object>()
                {
                    r.Name,
                    null,
                    Math.Round(ut.feetToMeters2(r.Area), 2),
                    Math.Round(ut.feetToMeters(r.Perimeter), 2),
                };
                roomsData.Add(roomData);
            }

            try
            {
                this.editData(sheet, getRange(row, roomsData.Count + row), roomsData);
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", e.ToString());
            }

        }

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

            return GetAllCircuitsDataFromCalcColumn(panel, "S");
        }

        public Dictionary<string, string> GetAllCircuitsBreakersAmps(ElectricalClasses.Panel panel)
        {
            return GetAllCircuitsDataFromCalcColumn(panel, "W");
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
            return GetAllCircuitsDataFromCalcColumn(panel, "Q");
        }

        public int GetDemandedLoadFromPanel ()
        {
            double demandedLoad = Convert.ToDouble(this.readData("Cálculo de Demanda", "G17:G17").Last().Last()) * 1000;

            return Convert.ToInt32(demandedLoad);
        }

        public int GetTotalLoadFromPanel()
        {
            int totalLoad = 0;
            foreach (object load in this.readData("Quadro de Carga", "X7:Z7").Last())
            {
                totalLoad += Convert.ToInt32(load);
            }
            return totalLoad;
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
