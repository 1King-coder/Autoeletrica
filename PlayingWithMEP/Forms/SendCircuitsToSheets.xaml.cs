﻿using System;
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

namespace AutoEletrica
{
    /// <summary>
    /// Interação lógica para UserControl1.xam
    /// </summary>
    public partial class SendCircuitsToSheets : Window
    {

        private readonly IRevitTask revitTask;
        private ECs.Panel selectedPanel;

        public IAsyncRelayCommand selectPanelBtnCmd { get; private set; }
        public IAsyncRelayCommand sendToSheetsCmd { get; private set; }

        public SendCircuitsToSheets(IRevitTask revitTask)
        {
            InitializeComponent();
            selectPanelBtnCmd = new AsyncRelayCommand(SelectPanelBtn_Click);
            sendToSheetsCmd = new AsyncRelayCommand(SendToSheetsBtn_Click);
            this.revitTask = revitTask;


        }

        private async Task SelectPanelBtn_Click()
        {
            this.Hide();
            this.selectedPanel = await revitTask.Run((uiapp) => {
                Document doc = uiapp.ActiveUIDocument.Document;
                Utils ut = new Utils(doc);

                Transaction selectTransaction = new Transaction(doc);
                Selection sel = uiapp.ActiveUIDocument.Selection;

                selectTransaction.Start("Selecionando Quadro de Distribuição");

                FamilyInstance element = ut.pickElement(sel, new SelectionFilterPanels());

                selectTransaction.Commit();

                ECs.Panel panel = new ECs.Panel(element, doc);

                return panel;
            });

            SelectedPanelLbl.Content = this.selectedPanel.Name;

            CircuitsDataTable.Items.Clear();

            foreach (ECs.Circuit circ in this.selectedPanel.AssignedCircuits)
            {

                CircuitsDataTable.Items.Add(circ);

            }

            this.Show();

        }

        private void SelectPanelBtn_Click_1(object sender, RoutedEventArgs e)
        {
            selectPanelBtnCmd.Execute(null);
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
                if (this.selectedPanel == null) { throw new ArgumentNullException("Selecione um quadro de distribuição primeiro!"); }
                planilha.SendCircuitsDataToSheets(this.selectedPanel);

                //await revitTask.Run((uiapp) =>
                //{
                //    planilha.SendRoomsToSheets(uiapp.ActiveUIDocument.Document);
                //});


            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ocorreu um erro", ex.ToString());
                return;
            }
            TaskDialog.Show("Sucesso", $"Os dados de todos os circuitos associados ao {this.selectedPanel.Name} foram inseridos na planilha com sucesso!");



        }

        private void verifyLinkBtn_Click(object sender, RoutedEventArgs e)
        {
            string spreadsheetLink = SheetsLinkTxtBox.Text.Trim();

            if (spreadsheetLink == null) { TaskDialog.Show("Link inválido!", "O link que você inseriu não é válido."); return; }

            string spreadsheetId = spreadsheetLink.Split('/')[5];

            try
            {
                GoogleSheetsManager gSheets = new GoogleSheetsManager(spreadsheetId);

                SpreadsheetTitleLbl.Content = gSheets.GetSpreadsheetTitle();
            }
            catch (Exception ex) { TaskDialog.Show("Sheets ID é inválido", "ID da planilha é inválido!"); }
        }
    }
}
