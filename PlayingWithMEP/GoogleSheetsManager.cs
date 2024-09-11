using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using System.IO;


namespace PlayingWithMEP
{
    internal class GoogleSheetsManager
    {
        private readonly string[] scopes = { SheetsService.Scope.Spreadsheets };

        private readonly string AppName = "Automacao Planilha de Dimensionamento eletrico";

        private string SpreadsheetId;

        public SheetsService service { get; set; }

        public GoogleSheetsManager(string spreadsheetId)
        {
            this.SpreadsheetId = spreadsheetId;

            this.service = GetSheetsService();

        }

        public SheetsService GetSheetsService()
        {
            GoogleCredential credential;

            
            
            using (var stream = new FileStream("C:\\Users\\vibar\\Source\\Repos\\1King-coder\\PlayingWithRevitMEP\\PlayingWithMEP\\credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
            }

            SheetsService service = new SheetsService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = this.AppName
            });
            
            return service;
        }

        public void writeData(string sheet, string range, List<object> data)
        {
            ValueRange valuesToSend = new ValueRange();

            List<IList<object>> dataMatrix = new List<IList<object>> { data };

            valuesToSend.Values = dataMatrix;

            var appendReq = this.service.Spreadsheets.Values.Append(valuesToSend, this.SpreadsheetId, $"{sheet}!{range}");
            appendReq.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            
            appendReq.Execute();

        
        }


    }
}
