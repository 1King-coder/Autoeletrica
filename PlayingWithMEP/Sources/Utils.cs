using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Electrical;
using System.Collections;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.DB.Architecture;
using ECs = PlayingWithMEP.ElectricalClasses;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.ObjectModel;
using System.Windows;
using Google.Apis.Sheets.v4.Data;
using System.Windows.Media.TextFormatting;
using System.Linq.Expressions;



namespace PlayingWithMEP
{
    internal class Utils
    {

        private Document doc;

        public Utils (Document doc)
        {
            this.doc = doc;
        }

        public FamilyInstance pickElement (Selection sel)
        {
            Reference pickedRef = null;


            try
            {
                pickedRef = sel.PickObject(ObjectType.Element, "Select the Eletric panel");
                
            }
            catch 
            {
                return null;
            }

            Element selectedPanel = this.doc.GetElement(pickedRef);

            return selectedPanel as FamilyInstance;
        }

        public double feetToMeters (double feetNum)
        {
            return feetNum / 3.281;
        }

        public double metersToFeet(double metersNum)
        {
            return metersNum / 0.3048;
        }

        public FamilyInstance pickElement(Selection sel, ISelectionFilter selectionFilter)
        {
            Reference pickedRef = null;


            try
            {
                pickedRef = sel.PickObject(ObjectType.Element, selectionFilter, "Select the Eletric panel");

            }
            catch
            {
                return null;
            }

            Element selectedPanel = this.doc.GetElement(pickedRef);

            return selectedPanel as FamilyInstance;
        }

        public List<FamilyInstance> pickElements(Selection sel)
        {
            List<Reference> pickedRefs = null;



            try
            {
                pickedRefs = sel.PickObjects(ObjectType.Element, "Select the Eletric panel").ToList();

            }
            catch
            {
                return null;
            }
            List<FamilyInstance> FIlist = new List<FamilyInstance>();
            foreach (Reference pickedRef in pickedRefs) 
            {
                FIlist.Add(this.doc.GetElement(pickedRef) as FamilyInstance);
            }


            return FIlist;
        }

        public List<FamilyInstance> pickElements(Selection sel, ISelectionFilter selectionFilter)
        {
            List<Reference> pickedRefs = null;



            try
            {
                pickedRefs = sel.PickObjects(ObjectType.Element, selectionFilter, "Select the Eletric panel").ToList();

            }
            catch
            {
                return null;
            }
            List<FamilyInstance> FIlist = new List<FamilyInstance>();
            foreach (Reference pickedRef in pickedRefs)
            {
                FIlist.Add(this.doc.GetElement(pickedRef) as FamilyInstance);
            }


            return FIlist;
        }

        public Reference pickElementRef(Selection sel)
        {
            Reference pickedRef = null;

            try
            {
                pickedRef = sel.PickObject(ObjectType.Element, "Select the Dispositive");

            }
            catch
            {
                return null;
            }

            return pickedRef;
        }

        public List<Conduit> GetConduitsFromPath (List<ElementId> cPath)
        {
            return cPath.ConvertAll(elId => this.doc.GetElement(elId) as Conduit);
        }
        public ElementId GetDispositiveCircuitShemeSymbolId (ElectricalClasses.Dispositive dispositive)
        {
            ECs.Circuit dispCirc = new ECs.Circuit(dispositive.EScircuit, doc);

            switch (dispCirc.scheme) 
            {
                case "F + N + T":
                    if (dispositive.dispType == "Lamp") return this.SymbolIdForIluminationDispositives("1 FN").Id;

                    return this.SymbolIdForPowerDispositives("1 FNT").Id;

                case "2F + T":
                    return this.SymbolIdForPowerDispositives("2FT").Id;

                case "2F + N + T":
                    return this.SymbolIdForPowerDispositives("2FNT").Id;

                case "3F + T":
                    return this.SymbolIdForPowerDispositives("3FT").Id;

                case "3F + N + T":
                    return this.SymbolIdForPowerDispositives("3FNT").Id;

                default:
                    return this.SymbolIdForPowerDispositives("1FN").Id;
            }
        }

        public FamilySymbol SymbolIdForPowerDispositives (string scheme) 
        {
            IEnumerable<FamilySymbol> CircuitFamilySymbols = new FilteredElementCollector(this.doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().Where(x => x.FamilyName.Equals("Fiação - Tags Eletrical Fixtures - Dispositivos"));


            return CircuitFamilySymbols.Where(x => x.Name.Equals(scheme)).First(); 
        }

        public FamilySymbol SymbolIdForPowerDispositives ()
        {
            IEnumerable<FamilySymbol> CircuitFamilySymbols = new FilteredElementCollector(this.doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().Where(x => x.FamilyName.Equals("Tag de N Circ Legenda Pt Tomada"));


            return CircuitFamilySymbols.Where(x => x.Name.Equals("Tag de N Circ Legenda Pt Tomada")).First();
        }

        public FamilySymbol SymbolIdForIluminationDispositives(string scheme)
        {
            IEnumerable<FamilySymbol> LuminaryFamilySymbol = new FilteredElementCollector(this.doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().Where(x => x.FamilyName.Equals("Fiação - Tags Lighting Fixture - Luminarias"));

            return LuminaryFamilySymbol.Where(x => x.Name.Equals(scheme)).First();
        }

        public FamilySymbol SymbolIdForSwitchesScheme ()
        {
            IEnumerable<FamilySymbol> LuminaryFamilySymbol = new FilteredElementCollector(this.doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().Where(x => x.FamilyName.Equals("Fiação - Tags Lighting Devices - Disp Iluminacao"));

            return LuminaryFamilySymbol.Where(x => x.Name.Equals("1 FR")).First();
        }

        public FamilySymbol SymbolIdForSwitches()
        {
            IEnumerable<FamilySymbol> LuminaryFamilySymbol = new FilteredElementCollector(this.doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().Where(x => x.FamilyName.Equals("Tag para Interruptor - Switch ID"));

            return LuminaryFamilySymbol.Where(x => x.Name.Equals("Tag para Interruptor - Switch ID")).First();
        }

        public FamilySymbol symbolIdForIluminationDispositivesOnRoof()
        {
            IEnumerable<FamilySymbol> LuminaryFamilySymbol = new FilteredElementCollector(this.doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().Where(x => x.FamilyName.Equals("Tag de Luminárias de Teto Circular"));

            return LuminaryFamilySymbol.Where(x => x.Name.Equals("Tag de Luminárias de Teto Circular")).First();
        }

        public FamilySymbol symbolIdForIluminationDispositivesOnWall()
        {
            IEnumerable<FamilySymbol> LuminaryFamilySymbol = new FilteredElementCollector(this.doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().Where(x => x.FamilyName.Equals("Tag de Luminárias na Parede"));

            return LuminaryFamilySymbol.Where(x => x.Name.Equals("Tag de Luminárias na Parede")).First();
        }

        public bool isDispositive (Element e)
        {
            return e.Category.Name == "Dispositivos elétricos";
        }

        public bool isLuminary (Element e)
        {
            return e.Category.Name == "Luminárias";
        }

        public bool isElectricEquipment(Element e)
        {
            return e.Category.Name == "Equipamento elétrico";
        }
        public bool isElectricalComponent (Element e )
        {
            return isDispositive (e) || isLuminary(e) || isElectricEquipment(e);
        }

        public List<ElectricalSystem> ESSetToList (ISet<ElectricalSystem> iset)
        {
            List<ElectricalSystem> newList = new List<ElectricalSystem>();

            foreach (ElectricalSystem el in iset)
            {
                newList.Add (el);
            }

            return newList;
        }

        public Dictionary<string, Parameter> ParamMapToDictonary (ParameterMap paramMap)
        {

            Dictionary<string, Parameter> newDictionary = new Dictionary<string, Parameter>();

            foreach (Parameter p in paramMap)
            {
                newDictionary.Add(p.Definition.Name, p);
            }

            return newDictionary;
        }

        public int loadStringToInt (string loadString)
        {
            return Convert.ToInt32(loadString.Remove(loadString.Length - 2));
        }

        public int voltageStringToInt (string voltageString)
        {
            return Convert.ToInt32(voltageString.Remove(voltageString.Length - 3));
        }

        public List<Connector> GetDispositiveUsedConnectors(ECs.Dispositive dispositive)
        {
            ConnectorSet allCons = dispositive.connectorManager.Connectors;
            List<Connector > AllconnectorsList = new List<Connector>();

            foreach (Connector con in allCons)
            {
                AllconnectorsList.Add (con);
            }

            return AllconnectorsList.Where((x) => !x.AllRefs.IsEmpty).ToList();

        }

        public List<Connector> GetConnectorListFromConnectorSet (ConnectorSet connectorSet)
        {
            List<Connector> connectorList = new List<Connector>();

            foreach (Connector con in connectorSet)
            {
                connectorList.Add (con);
            }

            return connectorList;
        }

        public List<Connector> GetPanelUsedConnectors(ElectricalEquipment EE)
        {
            ConnectorSet allCons = EE.ConnectorManager.Connectors;
            List<Connector> AllconnectorsList = new List<Connector>();

            foreach (Connector con in allCons)
            {
                AllconnectorsList.Add(con);
            }

            return AllconnectorsList.Where((x) => !x.AllRefs.IsEmpty).ToList();

        }

        public List<Connector> GetConduitElbowUsedConnectors(Conduit elbow)
        {
            ConnectorSet allCons = elbow.ConnectorManager.Connectors;
            List<int> AllConsId = new List<int>();

            List<Connector> AllconnectorsList = new List<Connector>();

            foreach (Connector con in allCons)
            {
                AllconnectorsList.Add(con);
            }

            return AllconnectorsList.Where((x) => !x.AllRefs.IsEmpty).ToList();
        }

        public bool isElbowConnectedToMultipleConduits (Conduit elbow)
        {
            return GetConduitElbowUsedConnectors(elbow).Count > 2;
        }

        public View GetViewByName (string name)
        {
            return new FilteredElementCollector(this.doc)
                .OfCategory(BuiltInCategory.OST_Views)
                .OfClass(typeof(View))
                .Where(x => x.Name.Equals(name))
                .Cast<View>().ToList().Last();
        }

        public string GetShemeToDiagrams (string sheme)
        {
            switch (sheme) 
            {
                case "F + N":
                    return "1 FN";

                case "F + N + T":
                    return "1 FNT";

                case "2F + T":
                    return "2 FT";

                case "2F + N + T":
                    return "2 FNT";

                case "3F + T":
                    return "3 FT";

                case "3F + N + T":
                    return "3 FNT";


                default:
                    return "1 FN";
            }

        }

        public List<Room> getRoomsFromLevel(Document document, Level level)
        {

            List<Element> Rooms = new FilteredElementCollector(document).OfClass(typeof(SpatialElement)).WhereElementIsNotElementType().Where(room => room.GetType() == typeof(Room)).ToList();

            return Rooms.Where(room => document.GetElement(room.LevelId) == level).Select(r => r as Room).ToList();
        }

        public FilledRegionType GetFilledRegionType(string name)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            return collector
              .OfClass(typeof(FilledRegionType)).Last() as FilledRegionType;
        }

        public Category GetLineStyleId (string name)
        {
            
            Category c = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Lines);
            
            return c.SubCategories.get_Item(name);
        }

        public FilledRegion CreateCircularFilledRegion(Document document, View view, double radius, XYZ centerPt)
        {

            Arc circle1 = Arc.Create(centerPt, radius, 0, Math.PI, XYZ.BasisX, XYZ.BasisY);
            Arc circle2 = Arc.Create(centerPt, radius, Math.PI, 2 * Math.PI, XYZ.BasisX, XYZ.BasisY);

            List<CurveLoop> profileloops = new List<CurveLoop>();
            CurveLoop profileloop = new CurveLoop();


            profileloop.Append(circle1);
            profileloop.Append(circle2);

            profileloops.Add(profileloop);

            FilledRegionType filledRegionType = this.GetFilledRegionType("Preto");

            if (filledRegionType == null)
            {
                string message = "Nenhum tipo de região preenchida disponível.";
                TaskDialog.Show("Error", message);
                return null;
            }
            
            // Cria a região preenchida
            FilledRegion filledRegion = FilledRegion.Create(doc, filledRegionType.Id, view.Id , profileloops);
            DetailElementOrderUtils.BringToFront(this.doc, view, filledRegion.Id);

            return filledRegion;
        }

        public void DrawRectangle (View view, XYZ topLeftCorner, XYZ bottomRightCorner)
        {
            XYZ topRigtCorner = new XYZ(bottomRightCorner.X, topLeftCorner.Y, 0);
            XYZ bottomLeftCorner = new XYZ(topLeftCorner.X, bottomRightCorner.Y, 0);

            List<Line> lines = new List<Line>() 
            {
                Line.CreateBound(topLeftCorner, topRigtCorner),
                Line.CreateBound(topRigtCorner, bottomRightCorner),
                Line.CreateBound(bottomRightCorner, bottomLeftCorner),
                Line.CreateBound(bottomLeftCorner, topLeftCorner)
            };

            lines.ForEach(line =>
            {
                DetailCurve curve = this.doc.Create.NewDetailCurve(view, line);
                curve.LineStyle = this.doc.GetElement(curve.GetLineStyleIds().Where(s => this.doc.GetElement(s).Name == "<Projeção>").Last());
            });



        }
    }
}
