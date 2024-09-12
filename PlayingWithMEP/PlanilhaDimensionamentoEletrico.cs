using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayingWithMEP
{
    internal class PlanilhaDimensionamentoEletrico : GoogleSheetsManager
    {
        public PlanilhaDimensionamentoEletrico(string spreadsheetId) : base(spreadsheetId) { }

        public void sendCircuitsDataToSheets()
        {
            // TODO: Implementar a escrita dos dados de circuitos na planilha
        }
        
    }
}
