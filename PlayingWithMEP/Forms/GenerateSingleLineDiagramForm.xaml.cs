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
    public partial class GenerateSingleLineDiagramForm : Window
    {
        
        private readonly IRevitTask revitTask;
        private ECs.Panel selectedPanel;
        private List<string> errMessages = new List<string>() 
        {
            "Valor inválido para disjuntor do padrão de entrada!",
            "Valor inválido para secção dos cabos do padrão de entrada!",
            "Valor inválido para disjuntor do quadro de distribuição!",
            "Valor inválido para secção dos cabos do quadro de distribuição",
            "Valor inválido para o número de polos do DR!",
            "Valor inválido para a corrente suportada do DR!",
            "Valor inválido para a corrente de proteção do DR!",
            "Valor inválido para a tensão nominal do DPS!",
            "Valor inválido para a corrente nominal do DPS!",
            "Valor inválido para a classe de proteção do DPS!"
        };

        public IAsyncRelayCommand selectPanelBtnCmd { get; private set; }
        public IAsyncRelayCommand genPanelBtnCmd { get; private set; }

        public GenerateSingleLineDiagramForm(IRevitTask revitTask)
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
                DisjuntorElecUtxtbox.Text,
                SeccionsElecUtxtbox.Text,
                DisjuntorPaneltxtbox.Text,
                SeccionsPaneltxtbox.Text,
                NumPolosDRtxtbox.Text,
                CorrenteDRtxtbox.Text,
                CorrenteProtDRtxtbox.Text,
                TensaoNomDPStxtbox.Text,
                CorrenteDPStxtbox.Text,
                ClasseDPStxtbox.Text,
            };
        }

        private int CheckFieldsContent ()
        {
            if (ShowElecUchkbox.IsChecked.Value)
            {
                try
                {
                    Convert.ToInt32(DisjuntorElecUtxtbox.Text);
                }
                catch (FormatException e)
                {
                    return 0;
                }
                try
                {
                    Convert.ToDouble(SeccionsElecUtxtbox.Text);
                }
                catch (FormatException e)
                {
                    return 1;
                }
            }
            
            try
            {
                Convert.ToInt32(DisjuntorPaneltxtbox.Text);
            }
            catch (FormatException e)
            {
                return 2;
            }
            try
            {
                Convert.ToDouble(SeccionsPaneltxtbox.Text);
            }
            catch (FormatException e)
            {
                return 3;
            }
            if (HasGeneralDRchkBox.IsChecked.Value)
            {
                try
                {
                    Convert.ToInt32(NumPolosDRtxtbox.Text);
                }
                catch (FormatException e)
                {
                    return 4;
                }
                try
                {
                    Convert.ToInt32(CorrenteDRtxtbox.Text);
                }
                catch (FormatException e)
                {
                    return 5;
                }
                try
                {
                    Convert.ToInt32(CorrenteProtDRtxtbox.Text);
                }
                catch (FormatException e)
                {
                    return 6;
                }
            }
            if (HasDPSchkbox.IsChecked.Value)
            {
                try
                {
                    Convert.ToInt32(TensaoNomDPStxtbox.Text);
                }
                catch (FormatException e)
                {
                    return 7;
                }
                try
                {
                    Convert.ToInt32(CorrenteDPStxtbox.Text);
                }
                catch (FormatException e)
                {
                    return 8;
                }
                if (!ClasseDPStxtbox.Text.ToLower().Contains("i") && !ClasseDPStxtbox.Text.ToLower().Contains("ii") && !ClasseDPStxtbox.Text.ToLower().Contains("iii"))
                {
                    return 9;
                }
            }
            
            return -1;

        }

        private ElectricalUtilityData SetUpElecetricalUtilityData(int correnteDisjuntor, double seccaoCabos)
        {
            ElectricalUtilityData elecUData = new ElectricalUtilityData();

            elecUData.CorrenteDisjuntor = correnteDisjuntor;
            elecUData.SeccaoCabos = seccaoCabos;

            return elecUData;
        }

        private PanelIdentifierData SetUpPanelIdentifierData(
            int CorrenteDisjuntorGeral,
            double SeccaoCabos,
            int DPSneutro,
            int HasDPS,
            int HasGeneralDR,
            string ClasseProtecaoDPS = "",
            int CorrenteNominalDPS = 0,
            int TensaoNominalDPS = 0,
            int CorrenteDR = 0,
            int CorrenteProtecaoDR = 0,
            int NumeroPolosDR = 0
           )
        {
            PanelIdentifierData panelIdentifierData = new PanelIdentifierData();

            panelIdentifierData.CorrenteDisjuntorGeral = CorrenteDisjuntorGeral;
            panelIdentifierData.SeccaoCabos = SeccaoCabos;
            if (HasGeneralDR == 1)
            {
                panelIdentifierData.HasGeneralDR = HasGeneralDR;
                panelIdentifierData.CorrenteDR = CorrenteDR;
                panelIdentifierData.CorrenteProtecaoDR = CorrenteProtecaoDR;
                panelIdentifierData.NumeroPolosDR = NumeroPolosDR;
            }
            if (HasDPS == 1)
            {
                panelIdentifierData.HasDPS = HasDPS;
                panelIdentifierData.DPSneutro = DPSneutro;
                panelIdentifierData.TensaoNominalDPS = TensaoNominalDPS;
                panelIdentifierData.ClasseDeProtecaoDPS = ClasseProtecaoDPS;
                panelIdentifierData.CorrenteNominalDPS = CorrenteNominalDPS;
            }

            return panelIdentifierData;
        }



        private async Task GenDiagramBtn_Click()
        {

            if (CheckFieldsContent() != -1)
            {
                TaskDialog.Show("Erro", errMessages[CheckFieldsContent()]);
                return;
            }

            string spreadsheetLink = SheetsLinkTxtBox.Text;

            if (spreadsheetLink == null) { TaskDialog.Show("Link inválido!", "O link que você inseriu não é válido."); return; }

            string spreadsheetId = spreadsheetLink.Split('/')[5];

            PlanilhaDimensionamentoEletrico planilha = new PlanilhaDimensionamentoEletrico(spreadsheetId);

            PanelIdentifierData panelIData = SetUpPanelIdentifierData(
                Convert.ToInt32(DisjuntorPaneltxtbox.Text),
                Convert.ToDouble(SeccionsPaneltxtbox.Text),
                (bool) DPSforNeutralUchkbox.IsChecked ? 1 : 0,
                (bool) HasDPSchkbox.IsChecked ? 1 : 0,
                (bool) HasGeneralDRchkBox.IsChecked ? 1 : 0,
                ClasseDPStxtbox.Text,
                Convert.ToInt32(CorrenteDPStxtbox.Text),
                Convert.ToInt32(TensaoNomDPStxtbox.Text),
                Convert.ToInt32(CorrenteDRtxtbox.Text),
                Convert.ToInt32(CorrenteProtDRtxtbox.Text),
                Convert.ToInt32(NumPolosDRtxtbox.Text)
                );

            ElectricalUtilityData elecUData = null;

            if (ShowElecUchkbox.IsChecked.Value)
            {
                elecUData = SetUpElecetricalUtilityData(Convert.ToInt32(DisjuntorElecUtxtbox.Text), Convert.ToDouble(SeccionsElecUtxtbox.Text));
            }

            this.Hide();
            try
            {
                await revitTask.Run((uiapp) =>
                {
                    Document doc = uiapp.ActiveUIDocument.Document;
                    Automations.GenerateDiagramsClass diagGen = new Automations.GenerateDiagramsClass(doc, planilha);
                    // uiapp.ActiveUIDocument.RequestViewChange(diagGen.singleLineView);
                    uiapp.ActiveUIDocument.ActiveView = diagGen.singleLineView;
                    this.Hide();
                    
                    Selection sel = uiapp.ActiveUIDocument.Selection;
                    XYZ insertionPt = null;

                    try
                    {
                        insertionPt = sel.PickPoint("Selecione onde gerar o diagrama");
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
                    {
                       insertionPt = new XYZ();
                    }

                    diagGen.GenSingleLineDiagramFromPanel(this.selectedPanel, panelIData, elecUData, insertionPt, (bool)ShowElecUchkbox.IsChecked);
                    this.Show();
                });
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Erro", ex.ToString());
                this.Show();
                return;
            }



            TaskDialog.Show("Sucesso", "Diagrama Unifilar gerado com Sucesso!");
            this.Show();


        }

        private void GenDiagramBtn_Click_1(object sender, RoutedEventArgs e)
        {
            genPanelBtnCmd.Execute(null);
        }
    }
}
