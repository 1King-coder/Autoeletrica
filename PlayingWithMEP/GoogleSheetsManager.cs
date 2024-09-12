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

            
            
            
            credential = GoogleCredential.FromJson(Properties.Resources.creds).CreateScoped(scopes);
            

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
            
            var response = appendReq.Execute();
        
        }

        public IList<IList<object>> readData(string sheet, string range)
        {
            var getRequest = this.service.Spreadsheets.Values.Get(this.SpreadsheetId, $"{sheet}!{range}");

            ValueRange response = getRequest.Execute();
            
            return response.Values;
        }

        public void editData(string sheet, string range, List<object> data)
        {
            ValueRange valuesToSend = new ValueRange();

            List<IList<object>> dataMatrix = new List<IList<object>> { data };
            
            valuesToSend.Values = dataMatrix;

            var updateReq = this.service.Spreadsheets.Values.Update(valuesToSend, this.SpreadsheetId, $"{sheet}!{range}");
            updateReq.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            
            var response = updateReq.Execute();
        }

        public void deleteData(string sheet, string range) {
            var deleteReq = this.service.Spreadsheets.Values.Clear(new ClearValuesRequest(), this.SpreadsheetId, $"{sheet}!{range}");
            
            var response = deleteReq.Execute();
        }


    }
}
