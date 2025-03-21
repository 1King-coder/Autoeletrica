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
using ECs = AutoEletrica.ElectricalClasses;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.ObjectModel;
using System.Windows;
using Google.Apis.Sheets.v4.Data;
using System.Windows.Media.TextFormatting;
using System.Linq.Expressions;
using Autodesk.Revit.Exceptions;



namespace AutoEletrica
{
    internal class Utils
    {

        private Document doc { get; set; }

        public Utils (Document doc)
        {
            this.doc = doc;
        }

        public Utils () { }

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
        public double feetToMeters2(double feetNum)
        {
            return feetNum / Math.Pow(3.281, 2);
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

        public FamilySymbol GetFamilySymbolByFamilyNameAndTypeName(string familyName, string typeName)
        {
            return new FilteredElementCollector(this.doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().Where(x => x.FamilyName.Equals(familyName))
                .Where(x => x.Name.Equals(typeName)).First();
        }

        public FamilySymbol SymbolIdForPowerDispositives (string scheme) 
        {
            return this.GetFamilySymbolByFamilyNameAndTypeName("Fiação - Tags Eletrical Fixtures - Dispositivos", scheme); 
        }

        public FamilySymbol SymbolIdForPowerDispositives ()
        {
            return this.GetFamilySymbolByFamilyNameAndTypeName("Tag de N Circ Legenda Pt Tomada", "Tag de N Circ Legenda Pt Tomada");
        }

        public FamilySymbol SymbolIdForPowerDispositivesBelow100load()
        {
            return this.GetFamilySymbolByFamilyNameAndTypeName("Tag Numero do circuito em Tomada", "Tag Numero do circuito em Tomada");
        }

        public FamilySymbol SymbolIdForIluminationDispositives(string scheme)
        {
            return this.GetFamilySymbolByFamilyNameAndTypeName("Fiação - Tags Lighting Fixture - Luminarias", scheme);
        }

        public FamilySymbol SymbolIdForSwitchesScheme ()
        {
            IEnumerable<FamilySymbol> LuminaryFamilySymbol = new FilteredElementCollector(this.doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().Where(x => x.Category.BuiltInCategory == BuiltInCategory.OST_LightingDeviceTags);

            return LuminaryFamilySymbol.Where(x => x.Name.Equals("1 FR")).First();
        }

        public FamilySymbol SymbolIdForSwitches()
        {
            return this.GetFamilySymbolByFamilyNameAndTypeName("Tag para Interruptor - Switch ID", "Tag para Interruptor - Switch ID");
        }

        public FamilySymbol symbolIdForIluminationDispositivesOnRoof()
        {
            return this.GetFamilySymbolByFamilyNameAndTypeName("Tag de Luminárias de Teto Circular", "Tag de Luminárias de Teto Circular");
        }

        public FamilySymbol symbolIdForIluminationDispositivesOnWall()
        {
            return this.GetFamilySymbolByFamilyNameAndTypeName("Tag de Luminárias na Parede", "Tag de Luminárias na Parede");
        }

        public FamilySymbol symbolIdForConduits()
        {
            return this.GetFamilySymbolByFamilyNameAndTypeName("Tag Diametro de Eletroduto", "Tag Diametro de Eletroduto");
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
            List<View> views = new FilteredElementCollector(this.doc)
                .OfCategory(BuiltInCategory.OST_Views)
                .OfClass(typeof(View))
                .Where(x => x.Name.Equals(name))
                .Cast<View>().ToList();

            return views.Count == 0 ? null : views.Last();
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

        public List<RevitLinkInstance> GetRevitLinks()
        {
            return new FilteredElementCollector(this.doc)
                .OfClass(typeof(RevitLinkInstance))
                .Cast<RevitLinkInstance>().ToList();
        }

        public List<Room> getRoomsFromProject(Document document)
        {
            List<Room> rooms = new FilteredElementCollector(document).OfClass(typeof(SpatialElement)).Cast<Room>().ToList();

            return rooms;
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

        public List<FamilyInstance> GetFamilyInstancesByFamilyName (string familyName, bool includeRevitLinks = false)
        {
            List<FamilyInstance> fminstances = new FilteredElementCollector(this.doc)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(x => x.Name == familyName)
                .ToList();

            return fminstances;
        }

        public List<IndependentTag> GetIndependentTagsByName (string name)
        {
            List<IndependentTag> indpTag = new FilteredElementCollector(this.doc)
                .OfClass(typeof(IndependentTag))
                .Cast<IndependentTag>()
                .Where(x => x.Name == name)
                .ToList();

            return indpTag;
        }

        private bool verifyConduitIsTaggable(Conduit conduit)
        {
            LocationCurve cLocCurve = conduit.Location as LocationCurve;
            Line cLine = cLocCurve.Curve as Line;
            int diameter = Convert.ToInt32(conduit.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).AsValueString());
            if (diameter == 25) return false;

            return this.feetToMeters(cLine.Length) > 0.3 && Math.Round(cLine.Direction.Z) == 0;
        }

        public List<Conduit> FilterTaggableConduits (List<Conduit> conduits)
        {
            return conduits.Where(x => this.verifyConduitIsTaggable(x)).ToList();
        }

        public List<IndependentTag> GetTagsInActiveView ()
        {
            return new FilteredElementCollector(this.doc, this.doc.ActiveView.Id)
                .OfClass(typeof(IndependentTag))
                .Cast<IndependentTag>()
                .ToList();
        }

        public List<Conduit> GetAllTaggableConduits()
        {
            return new FilteredElementCollector(this.doc)
                .OfClass(typeof(Conduit))
                .Cast<Conduit>()
                .Where(x => this.feetToMeters(x.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble()) > 2)
                .ToList();
        }

        public void ChangeTagByName (IndependentTag tag, string newFamilyName, string newTypeName)
        {
            try
            {
                using (Transaction t = new Transaction(this.doc, "Change Tag"))
                {
                    t.Start();
                    IndependentTag.Create(
                        this.doc,
                        this.GetFamilySymbolByFamilyNameAndTypeName(newFamilyName, newTypeName).Id,
                        tag.OwnerViewId,
                        tag.GetTaggedReferences().ToList().First(),
                        tag.HasLeader,
                        tag.TagOrientation,
                        tag.TagHeadPosition
                    );
                    this.doc.Delete(tag.Id);
                    t.Commit();
                }

            } catch (Autodesk.Revit.Exceptions.InvalidOperationException e) {
                TaskDialog.Show("Error", $"Erro ao realizar troca de familia: \n{e.ToString()}");
            }
        }

        public IList<T> Map<T> (List<T> list, Func<T, T> mapFunc)
        {
            IList<T> mappedList = new List<T>();

            foreach(T el in list)
            {
                mappedList.Add(mapFunc(el));
            }

            return mappedList;
        }

        public IList<G> Map<T, G> (IList<T> list, Func<T, G> mapFunc)
        {
            IList<G> mappedList = new List<G>();

            foreach (T el in list)
            {
                mappedList.Add(mapFunc(el));
            }

            return mappedList;
        }

        public List<ElectricalLoadClassification> GetAllElectricalLoadClassifications ()
        {
            return new FilteredElementCollector(this.doc)
                .OfClass(typeof(ElectricalLoadClassification))
                .Cast<ElectricalLoadClassification>()
                .ToList();
        }

        public List<WireType> GetAllWireType()
        {
            return new FilteredElementCollector(this.doc)
                .OfClass(typeof(WireType))
                .Cast<WireType>()
                .ToList();
        }

    }
}
