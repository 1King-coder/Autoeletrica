using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using AutoEletrica;
using ricaun.Revit.Mvvm;
using ricaun.Revit.UI.Tasks;
using AutoEletrica.Sources;
using ECs = AutoEletrica.ElectricalClasses;
using Autodesk.Revit.DB.Architecture;

namespace AutoEletrica
{
    /// <summary>
    /// Interação lógica para UserControl1.xam
    /// </summary>
    public partial class SendRoomsToSheets : Window
    {

        private readonly IRevitTask revitTask;
        private ECs.Panel selectedPanel;

        public IAsyncRelayCommand sendToSheetsCmd { get; private set; }

        public List<Room> projectRooms = new List<Room>();

        public SendRoomsToSheets(IRevitTask revitTask)
        {
            InitializeComponent();
            sendToSheetsCmd = new AsyncRelayCommand(SendToSheetsBtn_Click);
            this.revitTask = revitTask;
        }


        private void SendToSheetsBtn_Click_1 (object sender, RoutedEventArgs e)
        {
            sendToSheetsCmd.Execute(null);
        }

        private async Task SendToSheetsBtn_Click ()
        {
            string spreadsheetLink = SheetsLinkTxtBox.Text.Trim();

            if (spreadsheetLink == null) { TaskDialog.Show("Link inválido!", "O link que você inseriu não é válido."); return; }

            string spreadsheetId = spreadsheetLink.Split('/')[5];

            try
            {
                PlanilhaDimensionamentoEletrico planilha = new PlanilhaDimensionamentoEletrico(spreadsheetId);

                await revitTask.Run((uiapp) =>
                {
                    planilha.SendRoomsToSheets(this.projectRooms);
                });


            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ocorreu um erro", ex.ToString());
                return;
            }
            TaskDialog.Show("Sucesso", $"Os dados de todos os Ambientes foram inseridos na planilha com sucesso!");

        }

        private void verifyLinkBtn_Click(object sender, RoutedEventArgs e)
        {
            string spreadsheetLink = SheetsLinkTxtBox.Text;

            if (spreadsheetLink == null) { TaskDialog.Show("Link inválido!", "O link que você inseriu não é válido."); return; }

            spreadsheetLink = spreadsheetLink.Trim();


            string spreadsheetId = spreadsheetLink.Split('/')[5];

            try
            {
                GoogleSheetsManager gSheets = new GoogleSheetsManager(spreadsheetId);

                SpreadsheetTitleLbl.Content = gSheets.GetSpreadsheetTitle();
            }
            catch (Exception ex) { TaskDialog.Show("Sheets ID é inválido", "ID da planilha é inválido!"); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Utils ut = new Utils();

            this.projectRooms.ForEach((Room r) =>
            {
                RoomsDataGrid.Items.Add(new
                {
                    Name = r.Name,
                    Area = Math.Round(ut.feetToMeters2(r.Area), 2),
                    Perimeter = Math.Round(ut.feetToMeters(r.Perimeter), 2),
                    level = r.Level.Name
                });

            });
        }
    }
}
