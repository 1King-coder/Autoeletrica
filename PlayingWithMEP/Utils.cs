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
                    if (dispositive.dispType == "Lamp") return this.symbolIdForPowerDispositives("1 FN").Id;

                    return this.symbolIdForPowerDispositives("1 FNT").Id;

                case "2F + T":
                    return this.symbolIdForPowerDispositives("2FT").Id;

                case "2F + N + T":
                    return this.symbolIdForPowerDispositives("2FNT").Id;

                case "3F + T":
                    return this.symbolIdForPowerDispositives("3FT").Id;

                case "3F + N + T":
                    return this.symbolIdForPowerDispositives("3FNT").Id;

                default:
                    return this.symbolIdForPowerDispositives("1FN").Id;
            }
        }

        public FamilySymbol symbolIdForPowerDispositives (string scheme) 
        {
            IEnumerable<FamilySymbol> CircuitFamilySymbols = new FilteredElementCollector(this.doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().Where(x => x.FamilyName.Equals("Fiação - Tags Eletrical Fixtures - Dispositivos"));


            return CircuitFamilySymbols.Where(x => x.Name.Equals(scheme)).First(); 
        }

        public FamilySymbol symbolIdForIluminationDispositives (string scheme)
        {
            IEnumerable<FamilySymbol> LuminaryFamilySymbol = new FilteredElementCollector(this.doc).OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().Where(x => x.FamilyName.Equals("Fiação - Tags Lighting Fixture - Luminarias"));

            return LuminaryFamilySymbol.Where(x => x.Name.Equals(scheme)).First();
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

        public List<Connector> GetConduitElbowUsedConnectors(MEPModel elbow)
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

        public bool isElbowConnectedToMultipleConduits (MEPModel elbow)
        {
            return GetConduitElbowUsedConnectors(elbow).Count > 2;
        }
    }
}
