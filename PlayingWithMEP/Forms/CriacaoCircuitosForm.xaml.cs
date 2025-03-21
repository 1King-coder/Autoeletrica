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

        private readonly IRevitTask revitTask;

        public IAsyncRelayCommand SelecionaQD { get; private set; }
        public IAsyncRelayCommand AddDisp { get; private set; }
        public IAsyncRelayCommand RmDisp { get; private set; }

        private List<WireType> tiposFiacao { get; set; }
        private List<ElectricalEquipment> QDs { get; set; }
        private List<ElementId> preSelectedElements { get; set; }
        private ElectricalEquipment selectedQD { get; set; }

        private List<ElementId> SelectedElements { get; set; }

        public CriacaoCircuitosForm (
            IRevitTask revitTask, 
            List<WireType> tiposFiacao, 
            List<ElectricalEquipment> QDs, 
            List<ElementId> preSelectedElements
            )
        {
            InitializeComponent();
            SelecionaQD = new AsyncRelayCommand(SelecionaQDNoProjeto);
            AddDisp = new AsyncRelayCommand(AddDispositivo);
            RmDisp = new AsyncRelayCommand(RmDispositivo);
            this.revitTask = revitTask;
            this.tiposFiacao = tiposFiacao;
            this.QDs = QDs;
            this.preSelectedElements = preSelectedElements;
            this.SelectedElements = preSelectedElements;
        }

        private void LoadDataToForm()
        {
            TipoDeLigacaoComboBox.ItemsSource = new List<string> { "Monofásico", "Bifásico", "Trifásico" };
            TipoFiaçãoComboBox.ItemsSource = tiposFiacao;
            QDComboBox.ItemsSource = QDs;
        }

        private async Task SelecionaQDNoProjeto()
        {
            ElectricalEquipment selected = await revitTask.Run((UIApplication uiapp) =>
            {
               Document doc = uiapp.ActiveUIDocument.Document;
               Selection sel = uiapp.ActiveUIDocument.Selection;
               Utils ut = new Utils(doc);
               try
               {
                   ElectricalEquipment QD = ut.pickElement(sel, new SelectionFilterPanels()).MEPModel as ElectricalEquipment;
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
                QDComboBox.SelectedItem = this.selectedQD;
            }

        }

        

        private async Task AddDispositivo()
        {
            await revitTask.Run((UIApplication uiapp) =>
            {
                Document document = uiapp.ActiveUIDocument.Document;
                Selection sel = uiapp.ActiveUIDocument.Selection;

                Utils ut = new Utils(document);

                try
                {
                    IList<Reference> els = sel.PickObjects(ObjectType.Element, new SelectionFilterSelectedDispositives(this.SelectedElements), "Selecione dispositivos para adicionar ao circuito");

                    List<ElementId> elsIds = ut.Map<Reference, ElementId>(els, (Reference el) => el.ElementId).ToList();
                    this.SelectedElements = this.SelectedElements.Concat<ElementId>(elsIds);
                }
                catch (Exception)
                {
                    return;
                }

            });

        }

        private async Task RmDispositivo()
        {
            await revitTask.Run((UIApplication uiapp) =>
            {
                // Do something
            });

        }

        private void asyncFuncRunner_BTN(object sender, RoutedEventArgs e)
        {
            asyncCmd.Execute(null);
        }


    }
}
