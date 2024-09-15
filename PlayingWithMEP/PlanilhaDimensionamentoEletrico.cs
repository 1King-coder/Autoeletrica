using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayingWithMEP
{
    internal class PlanilhaDimensionamentoEletrico : GoogleSheetsManager
    {
        public ElectricalClasses.Panel panel { get; set; }
        public PlanilhaDimensionamentoEletrico(string spreadsheetId, ElectricalClasses.Panel panel) : base(spreadsheetId) 
        { 
            this.panel = panel; 
        }

        public async void SendCircuitsDataToSheets()
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

            foreach (ElectricalClasses.Circuit c in this.panel.AssignedCircuits)
            {
                List<object> circuitData_1 = new List<object>() { 
                    c.circuitNumber,
                    c.Name,
                };
                List<object> circuitData_2 = new List<object>() {
                    c.voltage,
                    c.scheme,
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

                await this.writeData(sheet1, ranges[0], circuitData_1);
                await this.writeData(sheet1, ranges[1], circuitData_2);
                await this.writeData(sheet1, ranges[2], circuitData_3);
                await this.writeData(sheet2, getCircuitCalcRange(row), circuitData_4);
                
                row++;
                
            }

        }
        
    }
}
