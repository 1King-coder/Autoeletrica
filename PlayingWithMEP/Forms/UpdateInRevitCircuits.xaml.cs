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
using AutoEletrica.Sources;
using ricaun.Revit.Mvvm;
using ricaun.Revit.UI.Tasks;
using ECs = AutoEletrica.ElectricalClasses;

namespace AutoEletrica
{
    /// <summary>
    /// Interação lógica para UserControl1.xam
    /// </summary>
    public partial class UpdateInRevitCircuits : Window
    {

        private readonly IRevitTask revitTask;
        private ECs.Panel selectedPanel;

        public IAsyncRelayCommand selectPanelBtnCmd { get; private set; }
        public IAsyncRelayCommand UpdateCircuitsBtnCmd { get; private set; }

        public UpdateInRevitCircuits(IRevitTask revitTask)
        {
            InitializeComponent();
            selectPanelBtnCmd = new AsyncRelayCommand(SelectPanelBtn_Click);
            UpdateCircuitsBtnCmd = new AsyncRelayCommand(UpdateBtn_Click);
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

        private void UpdateBtn_Click_1(object sender, RoutedEventArgs e)
        {
            UpdateCircuitsBtnCmd.Execute(null);
        }

        private async Task UpdateBtn_Click()
        {
            string spreadsheetLink = SheetsLinkTxtBox.Text.Trim();

            if (spreadsheetLink == null) { TaskDialog.Show("Link inválido!", "O link que você inseriu não é válido."); return; }

            string spreadsheetId = spreadsheetLink.Split('/')[5];

            
            
            try
            {
                PlanilhaDimensionamentoEletrico planilha = new PlanilhaDimensionamentoEletrico(spreadsheetId);
                if (this.selectedPanel == null) { throw new ArgumentNullException("Selecione um quadro de distribuição primeiro!"); }
                Dictionary<string, string> breakers = planilha.GetAllCircuitsBreakersAmps(this.selectedPanel);
                Dictionary<string, string> Seccions = planilha.GetAllCircuitsCableSeccion(this.selectedPanel);
                Dictionary<string, string> temperatureFactors = planilha.GetAllCircuitsTemperatureFactors(this.selectedPanel);
                Dictionary<string, string> groupFactors = planilha.GetAllCircuitsGroupFactors(this.selectedPanel);

                await revitTask.Run((uiapp) =>
                {
                    Transaction trans = new Transaction(uiapp.ActiveUIDocument.Document);
                    trans.Start("Updating circuits");

                    foreach (ECs.Circuit circuit in this.selectedPanel.AssignedCircuits)
                    {
                        circuit.CircuitObj.LookupParameter("Proteção do circuito").Set(Convert.ToInt64(breakers[circuit.circuitNumber]));
                        circuit.CircuitObj.LookupParameter("Seção do Condutor Adotado (mm²)").SetValueString(Seccions[circuit.circuitNumber]);

                        if (!circuit.Name.Contains("Reserva"))
                        {
                            circuit.CircuitObj.LookupParameter("L Considerado").Set(circuit.length);
                            circuit.CircuitObj.LookupParameter("FCA").Set(Convert.ToDouble(groupFactors[circuit.circuitNumber].Replace(',', '.'))/100);
                            circuit.CircuitObj.LookupParameter("FCT").Set(Convert.ToDouble(temperatureFactors[circuit.circuitNumber].Replace(',', '.')) / 100);
                        }
                        


                        foreach (ECs.Dispositive dispositive in circuit.dispositives)
                        {
                            if (Seccions[circuit.circuitNumber] == "2,5") 
                            { 
                                dispositive.dispositiveElement.LookupParameter("Seção do Condutor Adotado").Set(" ");
                                continue;
                            }

                            dispositive.dispositiveElement.LookupParameter("Seção do Condutor Adotado").Set(Seccions[circuit.circuitNumber]);
                        }

                    }

                    trans.Commit();

                });

                TaskDialog.Show("Sucesso", $"Os dados de todos os circuitos associados ao {this.selectedPanel.Name} foram atualizados com sucesso!");

            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ocorreu um erro", ex.ToString());
            }



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
