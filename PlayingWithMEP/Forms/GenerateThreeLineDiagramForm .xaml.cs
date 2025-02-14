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
using ECs = AutoEletrica.ElectricalClasses;
using Automations = AutoEletrica.ProjectAutomations;
using AutoEletrica.Sources;


namespace AutoEletrica
{
    /// <summary>
    /// Interação lógica para UserControl1.xam
    /// </summary>
    public partial class GenerateThreeLineDiagramForm : Window
    {
        
        private readonly IRevitTask revitTask;
        private ECs.Panel selectedPanel;
        private List<string> errMessages = new List<string>() 
        {
            "Valor inválido para disjuntor do quadro de distribuição!",
            "Valor inválido para secção dos cabos do quadro de distribuição",
        };

        public IAsyncRelayCommand selectPanelBtnCmd { get; private set; }
        public IAsyncRelayCommand genPanelBtnCmd { get; private set; }

        public GenerateThreeLineDiagramForm(IRevitTask revitTask)
        {
            InitializeComponent();
            selectPanelBtnCmd = new AsyncRelayCommand(SelectPanelBtn_Click);
            genPanelBtnCmd = new AsyncRelayCommand(GenDiagramBtn_Click);
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

        

        private void verifyLinkBtn_Click(object sender, RoutedEventArgs e)
        {
            string spreadsheetLink = SheetsLinkTxtBox.Text;

            if (spreadsheetLink == null) { TaskDialog.Show("Link inválido!", "O link que você inseriu não é válido."); return; }

            string spreadsheetId = spreadsheetLink.Split('/')[5];

            try
            {
                GoogleSheetsManager gSheets = new GoogleSheetsManager(spreadsheetId);

                SpreadsheetTitleLbl.Content = gSheets.GetSpreadsheetTitle();
            }
            catch (Exception ex) { TaskDialog.Show("Sheets ID é inválido", "ID da planilha é inválido!"); }
        }

        private List<string> GetAllFieldsContent ()
        {
            return new List<string>()
            {
                DisjuntorPaneltxtbox.Text,
                SeccionsPaneltxtbox.Text,
            };
        }

        private bool VerifyFieldsAreFilled ()
        {
            

            foreach (string field in GetAllFieldsContent())
            {
                if (string.IsNullOrEmpty(field)) { return false; }
            }

            return true;
            
        }

        private int CheckFieldsContent ()
        {
            
            try
            {
                Convert.ToInt32(DisjuntorPaneltxtbox.Text);
            }
            catch (FormatException e)
            {
                return 0;
            }
            try
            {
                Convert.ToDouble(SeccionsPaneltxtbox.Text);
            }
            catch (FormatException e)
            {
                return 1;
            }
            

            return -1;

        }

        private async Task GenDiagramBtn_Click()
        {
            if (!VerifyFieldsAreFilled())
            {
                TaskDialog.Show("Erro", "Preencha todos os campos!");
                return;
            }

            if (CheckFieldsContent() != -1)
            {
                TaskDialog.Show("Erro", errMessages[CheckFieldsContent()]);
                return;
            }

            string spreadsheetLink = SheetsLinkTxtBox.Text;

            if (spreadsheetLink == null) { TaskDialog.Show("Link inválido!", "O link que você inseriu não é válido."); return; }

            string spreadsheetId = spreadsheetLink.Split('/')[5];

            PlanilhaDimensionamentoEletrico planilha = new PlanilhaDimensionamentoEletrico(spreadsheetId);

            


            this.Hide();
            try
            {
                await revitTask.Run((uiapp) =>
                {
                    Document doc = uiapp.ActiveUIDocument.Document;
                    Automations.GenerateDiagramsClass diagGen = new Automations.GenerateDiagramsClass(doc, planilha);

                    ThreeLineDiagramBody threeLineDiagObj = new ThreeLineDiagramBody();

                    threeLineDiagObj.CorrenteDisjuntorGeral.Add("Corrente Disjuntor Geral", Convert.ToInt32(DisjuntorPaneltxtbox.Text));
                    threeLineDiagObj.SeccaoCabos.Add("Secção dos cabos", SeccionsPaneltxtbox.Text);
                    threeLineDiagObj.Tensao.Add("Tensão", Convert.ToInt32(TensaoDeAlimentacaotxtbox.Text));
                    threeLineDiagObj.CorrenteDeCurtoCircuito.Add("Corrente de Curto Circuito", Convert.ToInt32(CorrenteDeCCtxtbox.Text));
                    threeLineDiagObj.TemDR.Add("Tem DR", TemDRgeralChkbox.IsChecked.Value);
                    threeLineDiagObj.CorrenteDR.Add("Corrente DR", Convert.ToInt32(CorrenteSuportadaDRtxtbox.Text));
                    threeLineDiagObj.CorrenteDeProtecaoDR.Add("Corrente de Proteção DR", Convert.ToInt32(CorrenteDeProtecaoDRtxtbox.Text));
                    threeLineDiagObj.TemDPS.Add("Tem DPS", TemDPSchkbox.IsChecked.Value);
                    threeLineDiagObj.TemDPSParaNeutro.Add("DPS para o neutro", TemDPSParaNeutrochkbox.IsChecked.Value);
                    threeLineDiagObj.TensaoNominalDPS.Add("Tensão Nominal DPS", Convert.ToInt32(TensaoNominalDPStxtbox.Text));
                    threeLineDiagObj.CorrenteDeProtecaoDPS.Add("Corrente de proteção DPS", Convert.ToInt32(CorrenteDeProtecaoDPStxtbox.Text));
                    threeLineDiagObj.ClasseDeProtecaoDPS.Add("Classe DPS", ClasseDPStxtbox.Text);

                    diagGen.GenThreeLineDiagramFromPanel(selectedPanel, Convert.ToInt32(DisjuntorPaneltxtbox.Text), SeccionsPaneltxtbox.Text);
                    uiapp.ActiveUIDocument.ActiveView = diagGen.threeLineView;
                });
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Erro", ex.ToString());
                return;
            }



            TaskDialog.Show("Sucesso", "Diagrama Trifilar gerado com Sucesso!");
            this.Show();


        }

        private void GenDiagramBtn_Click_1(object sender, RoutedEventArgs e)
        {
            genPanelBtnCmd.Execute(null);
        }
    }
}
