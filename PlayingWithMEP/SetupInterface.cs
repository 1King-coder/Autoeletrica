

namespace PlayingWithMEP
{
    using Autodesk.Revit.UI;
    using System.Reflection;
    using System.Windows.Media.Imaging;
    using Autodesk.Revit.DB.Events;
    using System;
    using Autodesk.Revit.ApplicationServices;
    using ricaun.Revit.UI;

    public class SetupInterface
    {
        
        public SetupInterface() { }

        public void Initialize(UIControlledApplication application)
        {
            string tabName = "AutoEletrica";
            string panelAnnotationName = "Automatizações";

            application.CreateRibbonTab(tabName);
            RibbonPanel panelAnnotation = application.CreateRibbonPanel(tabName, panelAnnotationName);

            var SendCircuitsToSheetsbtnData = new PushButtonData("Enviar dados dos circuitos para Planilha", "Envia dados\nCircuitos", Assembly.GetExecutingAssembly().Location, "PlayingWithMEP.SendingCircuitsDataToSheets")
            {
                ToolTipImage = new BitmapImage(new System.Uri(@"C:\Users\vibar\source\repos\PlayingWithMEP\PlayingWithMEP\sheetsIcon.ico"))
                , ToolTip = "Essa ferramenta é usada para enviar dados dos circuitos de um painel para planilha de dimensionamento"
            };

            SendCircuitsToSheetsbtnData.SetLargeImage(new BitmapImage(new System.Uri(@"C:\Users\vibar\source\repos\PlayingWithMEP\PlayingWithMEP\sheetsIcon.ico")));

            var GenerateSingleLineDiagrambtnData = new PushButtonData("Gerar Diagrama Unifilar", "Diagrama \nUnifilar", Assembly.GetExecutingAssembly().Location, "PlayingWithMEP.DiagramaUnifilar")
            {
                ToolTipImage = new BitmapImage(new System.Uri(@"C:\Users\vibar\source\repos\PlayingWithMEP\PlayingWithMEP\disjuntorUnifilar.png"))
                ,
                ToolTip = "Essa ferramenta é usada para gerar o diagrama unifilar de um quadro de distribuição selecionado."
            };

            GenerateSingleLineDiagrambtnData.SetLargeImage(new BitmapImage(new System.Uri(@"C:\Users\vibar\source\repos\PlayingWithMEP\PlayingWithMEP\disjuntorUnifilar.png")));



            PushButton SendCircuitsToSheetsbtn = panelAnnotation.AddItem(SendCircuitsToSheetsbtnData) as PushButton;
            PushButton GenerateSingleLineDiagrambtn = panelAnnotation.AddItem(GenerateSingleLineDiagrambtnData) as PushButton;



        }
    }
}
