

namespace AutoEletrica
{
    using Autodesk.Revit.UI;
    using System.Reflection;
    using System.Windows.Media.Imaging;
    using Autodesk.Revit.DB.Events;
    using System;
    using Autodesk.Revit.ApplicationServices;
    using ricaun.Revit.UI;
    using System.IO;
    using AutoEletrica.Properties;

    public class SetupInterface
    {
        
        public SetupInterface() { }

        public void Initialize(UIControlledApplication application)
        {
            
            string tabName = "AutoEletrica";
            string panelAnnotationName = "Automatizações";

            string thisdirectorypath = Assembly.GetExecutingAssembly().Location;

            string sheetsIcoPath = Path.Combine(Path.GetDirectoryName(thisdirectorypath), "Icons", "sheetsIcon.ico");
            string singleLineDPath = Path.Combine(Path.GetDirectoryName(thisdirectorypath), "Icons", "disjuntorUnifilar.png");
            string threeLineDPath = Path.Combine(Path.GetDirectoryName(thisdirectorypath), "Icons", "disjuntorTrifilar.png");
            string singleLineFullDPath = Path.Combine(Path.GetDirectoryName(thisdirectorypath), "Icons", "DiagramaUni.png");
            string threeLineFullDPath = Path.Combine(Path.GetDirectoryName(thisdirectorypath), "Icons", "DiagramaTri.png");
            string identifyCircsPath = Path.Combine(Path.GetDirectoryName(thisdirectorypath), "Icons", "indentificadorEsquema.png");
            string updateCircsPath = Path.Combine(Path.GetDirectoryName(thisdirectorypath), "Icons", "updateCircuits.png");
            string associateSwitchesPath = Path.Combine(Path.GetDirectoryName(thisdirectorypath), "Icons", "associateSwitches.png");


            application.CreateRibbonTab(tabName);
            RibbonPanel panelAnnotation = application.CreateRibbonPanel(tabName, panelAnnotationName);

            var SendCircuitsToSheetsbtnData = new PushButtonData("Enviar dados dos circuitos para Planilha", "Envia dados\nCircuitos", Assembly.GetExecutingAssembly().Location, "AutoEletrica.SendingCircuitsDataToSheets")
            {
                ToolTipImage = new BitmapImage(new System.Uri(sheetsIcoPath))
                , ToolTip = "Essa ferramenta é usada para enviar dados dos circuitos de um painel para planilha de dimensionamento"
            };

            SendCircuitsToSheetsbtnData.SetLargeImage(new BitmapImage(new System.Uri(sheetsIcoPath)));

            var GenerateSingleLineDiagrambtnData = new PushButtonData("Gerar Diagrama Unifilar", "Diagrama \nUnifilar", Assembly.GetExecutingAssembly().Location, "AutoEletrica.DiagramaUnifilar")
            {
                ToolTipImage = new BitmapImage(new System.Uri(singleLineFullDPath))
                ,
                ToolTip = "Essa ferramenta é usada para gerar o diagrama unifilar de um quadro de distribuição selecionado."
            };

            GenerateSingleLineDiagrambtnData.SetLargeImage(new BitmapImage(new System.Uri(singleLineDPath)));

            var GenerateThreeLineDiagrambtnData = new PushButtonData("Gerar Diagrama Trifilar", "Diagrama \nTrifilar", Assembly.GetExecutingAssembly().Location, "AutoEletrica.DiagramaTrifilar")
            {
                ToolTipImage = new BitmapImage(new System.Uri(threeLineFullDPath))
                ,
                ToolTip = "Essa ferramenta é usada para gerar o diagrama Trifilar de um quadro de distribuição selecionado."
            };

            GenerateThreeLineDiagrambtnData.SetLargeImage(new BitmapImage(new System.Uri(threeLineDPath)));


            var IdentifyElectricalElementsbtnData = new PushButtonData("Identificar dispositivos", "Identificar \nDispositivo", Assembly.GetExecutingAssembly().Location, "AutoEletrica.IdentifyDispositives")
            {
                
                ToolTip = "Essa ferramenta é usada para identificar o esquema de ligação de um dispositivo ou circuito e potências de vários dispositivos de um painel. \nSelecionar um dispositivo -> identifica o esquema do circuito \nSelecionar um quadro de distribuição -> Identifica o circuito e potência dos dispositivos conectados no quadro. "
            };

            IdentifyElectricalElementsbtnData.SetLargeImage(new BitmapImage(new System.Uri(identifyCircsPath)));

            var UpdateCircuitsInRevitbtnData = new PushButtonData("Atualizar circuitos", "Atualizar \nCircuitos", Assembly.GetExecutingAssembly().Location, "AutoEletrica.UpdateInRevitCircuitsFromPanel")
            {
                
                ToolTip = "Essa ferramenta é usada para recolher os dados dos circuitos dimensionados na planilha de dimensionamento e trazer dados como corrente do disjuntor e secção dos cabos "
            };

            UpdateCircuitsInRevitbtnData.SetLargeImage(new BitmapImage(new System.Uri(updateCircsPath)));

            var AssociateSwitchesbtnData = new PushButtonData("Associar comandos", "Associar \nComandos", Assembly.GetExecutingAssembly().Location, "AutoEletrica.AssociaComandosAInterruptores")
            {

                ToolTip = "Essa ferramenta é usada para associar comandos aos interruptores associados ao QD selecionado (1a = QD 1 comando a)"
            };

            AssociateSwitchesbtnData.SetLargeImage(new BitmapImage(new System.Uri(associateSwitchesPath)));



            PushButton SendCircuitsToSheetsbtn = panelAnnotation.AddItem(SendCircuitsToSheetsbtnData) as PushButton;
            PushButton GenerateSingleLineDiagrambtn = panelAnnotation.AddItem(GenerateSingleLineDiagrambtnData) as PushButton;
            PushButton GenerateThreeLineDiagrambtn = panelAnnotation.AddItem(GenerateThreeLineDiagrambtnData) as PushButton;
            PushButton IdentifyElectricalElementsbtn = panelAnnotation.AddItem(IdentifyElectricalElementsbtnData) as PushButton;
            PushButton UpdateCircuitsInRevitbtn = panelAnnotation.AddItem(UpdateCircuitsInRevitbtnData) as PushButton;
            PushButton AssociateSwitchesbtn = panelAnnotation.AddItem(AssociateSwitchesbtnData) as PushButton;



        }
    }
}
