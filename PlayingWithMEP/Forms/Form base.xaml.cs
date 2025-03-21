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
    public partial class FormName : Window
    {

        private readonly IRevitTask revitTask;

        public IAsyncRelayCommand asyncCmd { get; private set; }

        public FormName(IRevitTask revitTask)
        {
            InitializeComponent();
            asyncCmd = new AsyncRelayCommand(asyncCmdFunc);
            this.revitTask = revitTask;


        }

        private async Task asyncCmdFunc()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
