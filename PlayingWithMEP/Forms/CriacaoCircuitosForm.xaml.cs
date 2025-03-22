using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
using Autodesk.Revit.DB.Electrical;
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
    public partial class CriacaoCircuitosForm : Window
    {
        private Document document;
        private readonly IRevitTask revitTask;

        public IAsyncRelayCommand SelecionaQD { get; private set; }
        public IAsyncRelayCommand AddDisp { get; private set; }
        public IAsyncRelayCommand RmDisp { get; private set; }

        private List<WireType> tiposFiacao { get; set; }
        private List<FamilyInstance> QDs { get; set; }
        private List<ElementId> preSelectedElements { get; set; }
        private FamilyInstance selectedQD { get; set; }

        private List<ElementId> SelectedElements { get; set; }

        private Dictionary<string, ElementId> QDsIdByName { get; set; }
        private Dictionary<string, WireType> WireTypesByName { get; set; }
        private int totalLoad { get; set; }
        private int nominalCurrent { get; set; }
        private int adjustedCurrent = 0;
        private int selectedVoltage { get; set; }
        private Dictionary<string, int> voltages { get; set; }


        public CriacaoCircuitosForm (
            IRevitTask revitTask,
            Document document,
            List<WireType> tiposFiacao, 
            List<FamilyInstance> QDs, 
            List<ElementId> preSelectedElements
            )
        {
            InitializeComponent();
            this.document = document;
            SelecionaQD = new AsyncRelayCommand(SelecionaQDNoProjeto);
            AddDisp = new AsyncRelayCommand(AddDispositivo);
            RmDisp = new AsyncRelayCommand(RmDispositivo);
            this.revitTask = revitTask;
            this.tiposFiacao = tiposFiacao;
            this.QDs = QDs;
            this.preSelectedElements = preSelectedElements;
            this.SelectedElements = preSelectedElements;
            this.voltages = new Dictionary<string, int> { { "127V", 127 }, { "220V", 220 }, { "380V", 380 } };
            this.QDsIdByName = new Dictionary<string, ElementId>();
            this.WireTypesByName = new Dictionary<string, WireType>();

            this.selectedVoltage = 127;

            QDs.ForEach((FamilyInstance el) =>
            {
                QDsIdByName.Add(el.Name, el.Id);
            });

            tiposFiacao.ForEach((WireType el) =>
            {
                WireTypesByName.Add(el.Name, el);
            });

            this.LoadDataToForm();
        }

        private int CalcTotalLoad()
        {
            if (this.SelectedElements.Count == 0) { return 0; }
            return this.SelectedElements.Sum((ElementId el) => Utils.GetDispositiveApparentLoad(document.GetElement(el) as FamilyInstance));
        }

        private void LoadDataToForm()
        {
            TipoDeLigacaoComboBox.ItemsSource = new List<string> { "Monofásico", "Bifásico", "Trifásico" };
            TipoDeLigacaoComboBox.SelectedIndex = 0;
            TipoFiaçãoComboBox.ItemsSource = WireTypesByName.Keys;
            TipoFiaçãoComboBox.SelectedIndex = 0;
            QDComboBox.ItemsSource = QDsIdByName.Keys;
            TensaoComboBox.ItemsSource = new List<string> { "127V", "220V", "380V" };
            TensaoComboBox.SelectedIndex = 0;

            QtdeElemTxtBox.Text = this.SelectedElements.Count().ToString();

            this.totalLoad = this.CalcTotalLoad();

            PotAparenteTxtBox.Text = this.totalLoad.ToString();

            this.nominalCurrent = (int) Math.Ceiling((double)this.totalLoad / this.selectedVoltage);
            CorrenteNomTxtBox.Text = this.nominalCurrent.ToString();

            if (GlobalParametersManager.AreGlobalParametersAllowed(document))
            {
                double fatorCorrecao = Utils.GetGlobalParameterDoubleValue(document, "FATOR_DE_CORRECAO_DIMENSIONAMENTO_DE_DISJUNTORES");
                this.adjustedCurrent = Convert.ToInt32(Math.Ceiling(this.nominalCurrent * (1 + fatorCorrecao)));
            }
            CorrenteCorrigidaTxtBox.Text = this.adjustedCurrent.ToString();

        }

        private async Task SelecionaQDNoProjeto()
        {
            this.Hide();
            FamilyInstance selected = await revitTask.Run((UIApplication uiapp) =>
            {
               Document doc = uiapp.ActiveUIDocument.Document;
               Selection sel = uiapp.ActiveUIDocument.Selection;
               Utils ut = new Utils(doc);
               try
               {
                   FamilyInstance QD = ut.pickElement(sel, new SelectionFilterPanels());
                   return QD;
               }
               catch (Exception)
               {
                   return null;
               }
            });

            if (selected != null)
            {
                this.selectedQD = selected;
                QDComboBox.SelectedItem = this.selectedQD.Name;
            }
            this.Show();
        }

        private async Task AddDispositivo()
        {
            this.Hide();
            await revitTask.Run((UIApplication uiapp) =>
            {
                Document document = uiapp.ActiveUIDocument.Document;
                Selection sel = uiapp.ActiveUIDocument.Selection;

                Utils ut = new Utils(document);

                try
                {
                    IList<Reference> els = sel.PickObjects(ObjectType.Element, new SelectionFilterNoSelectedDispositives(this.SelectedElements), "Selecione dispositivos para adicionar ao circuito");

                    IList<ElementId> elsIds = ut.Map<Reference, ElementId>(els, (Reference el) => el.ElementId);

                    this.SelectedElements = this.SelectedElements.Concat<ElementId>(elsIds).ToList();
                }
                catch (Exception)
                {
                    this.Show();
                    return;
                }
                this.reloadFormData();
                this.Show();
                

            });
        }

        private async Task RmDispositivo()
        {
            this.Hide();
            await revitTask.Run((UIApplication uiapp) =>
            {
                Document document = uiapp.ActiveUIDocument.Document;
                Selection sel = uiapp.ActiveUIDocument.Selection;

                Utils ut = new Utils(document);

                try
                {
                    IList<Reference> els = sel.PickObjects(ObjectType.Element, new SelectionFilterOnlySelectedDispositives(this.SelectedElements), "Selecione dispositivos para adicionar ao circuito");

                    IList<ElementId> elsIds = ut.Map<Reference, ElementId>(els, (Reference el) => el.ElementId);

                    elsIds.ToList().ForEach( (ElementId elId) =>
                    {
                       this.SelectedElements.Remove(elId);
                    });
                }
                catch (Exception)
                {
                    this.Show();
                    return;
                }
                this.reloadFormData();
                this.Show();


            });

        }

        private void reloadFormData()
        {
            this.totalLoad = this.CalcTotalLoad();
            PotAparenteTxtBox.Text = this.totalLoad.ToString();

            QtdeElemTxtBox.Text = this.SelectedElements.Count().ToString();

            this.nominalCurrent = (int)Math.Ceiling((double)this.totalLoad / this.selectedVoltage);
            this.adjustedCurrent = Convert.ToInt32(
                Math.Ceiling(
                    this.nominalCurrent * (1 + Utils.GetGlobalParameterDoubleValue(document, "FATOR_DE_CORRECAO_DIMENSIONAMENTO_DE_DISJUNTORES"))
                ));
            CorrenteCorrigidaTxtBox.Text = this.adjustedCurrent.ToString();
            CorrenteNomTxtBox.Text = this.nominalCurrent.ToString();
        }

        private void AdicionarDispBtn_Click(object sender, RoutedEventArgs e)
        {
            AddDisp.Execute(null);
        }

        private void RemoverDispBtn_Click(object sender, RoutedEventArgs e)
        {
            RmDisp.Execute(null);
        }

        private void CriarCircBtn_Click(object sender, RoutedEventArgs e)
        {
            FamilyInstance qd = this.selectedQD;
            WireType tipoFiação = this.WireTypesByName[TipoFiaçãoComboBox.SelectedItem.ToString()];
            int TemDR = TemDRChkBox.IsChecked == true ? 1 : 0;
            string nomeCircuito = NomeCircuitoTxtbox.Text;
            if (nomeCircuito == "")
            {
                TaskDialog.Show("Erro", "Nome do circuito não pode ser vazio");
                return;
            }

            int correnteDR = 0;
            if (TemDR == 1)
            {
                correnteDR = Convert.ToInt32(CorrenteDRTxtBox.Text);

            }

            try
            {
                if (this.SelectedElements == null || this.SelectedElements.Count == 0)
                {
                    TaskDialog.Show("Erro", "Nenhum dispositivo selecionado");
                    return;
                }
                ProjectAutomations.CreateCircuit(document, nomeCircuito, tipoFiação, TemDR, correnteDR, this.SelectedElements);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Erro", ex.Message);
                return;
            }

            TaskDialog.Show("Sucesso", $"Circuito {nomeCircuito} criado com sucesso");
            this.clearFormAfterCreation();
        }

        private void SelecionarQDBtn_Click(object sender, RoutedEventArgs e)
        {
            SelecionaQD.Execute(null);
        }
        private void TensaoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.voltages[TensaoComboBox.SelectedItem.ToString()] != this.selectedVoltage)
            {
                this.selectedVoltage = this.voltages[TensaoComboBox.SelectedItem.ToString()];
                this.reloadFormData();
            }
        }

        private void clearFormAfterCreation()
        {
            this.SelectedElements = new List<ElementId>();
            this.selectedQD = null;
            this.selectedVoltage = 127;
            TipoFiaçãoComboBox.SelectedIndex = 0;
            TipoDeLigacaoComboBox.SelectedIndex = 0;
            TensaoComboBox.SelectedIndex = 0;
            QDComboBox.SelectedIndex = 0;
            NomeCircuitoTxtbox.Text = "";
            CorrenteDRTxtBox.Text = "";
            TemDRChkBox.IsChecked = false;
            this.reloadFormData();
        }

        private void QDComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedQD = this.document.GetElement(this.QDsIdByName[QDComboBox.SelectedItem.ToString()]) as FamilyInstance;
        }
    }
}
