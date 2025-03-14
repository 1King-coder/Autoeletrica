using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace AutoEletrica
{
    internal class ElectricalClasses
    {
        public class Panel
        {
            public Panel(FamilyInstance panelElement, Document doc)
            {
                Utils u = new Utils(doc);

                ElectricalEquipment panelEE = panelElement.MEPModel as ElectricalEquipment;

                this.panelElement = panelElement;

                this.PanelObj = panelEE;

                this.AssignedCircuits = SortCircuitsByNumber(getCircuits(panelEE, doc));

                this.totalLoad = getPanelTotalLoad(this.AssignedCircuits);

                this.demandedLoad = 0;

                this.Name = panelElement.Name;

                this.location = this.panelElement.Location as LocationPoint;

                this.UsedConnectors = u.GetPanelUsedConnectors(this.PanelObj);

                this.numOfPoles = panelElement.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NUMPHASES_PARAM).AsInteger();
                this.numOfNeutrals = panelElement.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NUMWIRES_PARAM).AsInteger() - numOfPoles; // gambiarra
                this.numOfGrounds = 1; // gambiarra

                this.scheme = this.GetScheme();

            }

            public int getPanelTotalLoad(List<Circuit> panelCircuits)
            {
                int totalLoad = 0;
                foreach (Circuit circuit in panelCircuits)
                {
                    totalLoad += circuit.apparentload;
                }

                return totalLoad;
            }

            public List<Circuit> getCircuits(ElectricalEquipment panelEE, Document doc)
            {
                List<Circuit> circuitList = new List<Circuit>();

                foreach (ElectricalSystem eS in panelEE.GetAssignedElectricalSystems())
                {
                    Circuit c = new Circuit(eS, doc);

                    circuitList.Add(c);
                }

                return circuitList;
            }

            private string schemeNumToString(int num)
            {
                return num == 1 ? "" : num.ToString();
            }

            public string GetScheme()
            {
                string poles = $"{schemeNumToString(this.numOfPoles)}F";
                string neutrals = !this.numOfNeutrals.Equals(0) ? $" + {schemeNumToString(this.numOfNeutrals)}N" : "";
                string grounds = !this.numOfGrounds.Equals(0) ? $" + {schemeNumToString(this.numOfGrounds)}T" : "";

                string scheme = $"{poles}{neutrals}{grounds}";

                return scheme;
            }

            public List<Circuit> SortCircuitsByNumber (List<Circuit> Circuits)
            {
                List<Circuit> sortedCircuits = new List<Circuit> (Circuits.OrderBy((c) => {
                    String circNum = c.circuitNumber;
                    circNum = circNum.Contains("-") ? circNum.Split('-')[1] : circNum;
                    if (circNum.Split(',').ToList().Count() > 1) { return Convert.ToInt32(circNum.Split(',')[0]); } else { return Convert.ToInt32(circNum); };
                }));

                return sortedCircuits;
            }

            public string scheme { get; set; }
            public int numOfNeutrals { get; set; }
            public int numOfGrounds { get; set; }

            public int numOfPoles { get; set; }

            public FamilyInstance panelElement { get; set; }

            public ElectricalEquipment PanelObj { get; set; }

            public List<Circuit> AssignedCircuits { get; set; }

            public int totalLoad { get; set; }

            public double demandedLoad { get; set; }

            public string Name { get; set; }

            public LocationPoint location {  get; set; }

            public List<Connector> UsedConnectors { get; set; }
        }

        public class Circuit
        {
            public Circuit(ElectricalSystem ES, Document doc)
            {

                Utils u = new Utils(doc);

                this.CircuitObj = ES;

                this.circuitNumber = ES.CircuitNumber;

                this.Name = ES.LoadName;

                string apparentLoadString = ES.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_LOAD).AsValueString();

                this.apparentload = u.loadStringToInt(apparentLoadString);

                this.length = GetLongerPath(ES, doc);

                this.typeId = ES.GetTypeId();

                this.levelId = ES.LevelId;

                this.dispositives = getDispositives(ES.Elements, doc);

                this.voltage = u.voltageStringToInt(ES.get_Parameter(BuiltInParameter.RBS_ELEC_VOLTAGE).AsValueString());

                this.numOfDispositivesByLoad = GetNumOfDispositivesByLoad();
                    
                this.numOfPoles = ES.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM).AsInteger();
                this.numOfNeutrals = ES.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_NEUTRALS_PARAM).AsInteger();
                this.numOfGrounds = ES.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_GROUNDS_PARAM).AsInteger();

                this.phaseALoad = ES.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_LOAD_PHASEA).AsValueString().Replace("VA", "").Trim();
                this.phaseBLoad = ES.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_LOAD_PHASEB).AsValueString().Replace("VA", "").Trim();
                this.phaseCLoad = ES.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_LOAD_PHASEC).AsValueString().Replace("VA", "").Trim();

                this.scheme = GetScheme();

                this.isNotReserveCircuit = this.Name.Contains("Reserva") ? 0 : 1;

                this.TemDR = ES.LookupParameter("Tem DR").AsInteger();
                this.CorrenteSuportadaDR = ES.LookupParameter("Corrente Suportada DR").AsInteger();
                this.CorrenteDeProtecaoDR = ES.LookupParameter("Corrente de proteção DR").AsInteger();
                this.TemNeutro = ES.LookupParameter("Circuito com Neutro").AsInteger();
                this.TemTerra = ES.LookupParameter("Circuito com Terra").AsInteger();
                this.NumeroDePolosDR = this.numOfPoles == 1 ? 2 : (this.numOfPoles == 2 && this.TemNeutro == 1 ? 4 : (this.numOfPoles == 3 ? 4 : 2));


            }

            public static double GetLongerPath(ElectricalSystem ES, Document doc)
            {
                Transaction changeTrans1 = new Transaction(doc);
                Transaction changeTrans2 = new Transaction(doc);

                changeTrans1.Start("Changing Circuit " + ES.CircuitNumber + " path mode to AllDevices");
                ES.CircuitPathMode = ElectricalCircuitPathMode.AllDevices;
                double length1 = Math.Round(ES.Length / 3.281, 0);
                changeTrans1.Commit();

                changeTrans2.Start("Changing Circuit " + ES.CircuitNumber + " path mode to FarthestDevice");
                ES.CircuitPathMode = ElectricalCircuitPathMode.FarthestDevice;
                double lenght2 = Math.Round(ES.Length / 3.281, 0);
                changeTrans2.Commit();


                return Math.Max(length1, lenght2);
            }

            public List<Dispositive> getDispositives (ElementSet dispositivesSet, Document doc)
            {
                List<Dispositive> disps = new List<Dispositive>();

                foreach (Element d in dispositivesSet)
                {

                    Dispositive disp = new Dispositive(d, doc);

                    disps.Add(disp);
                }

                return disps;
            }

            public string phaseALoad {  get; set; }
            public string phaseBLoad {  get; set; }
            public string phaseCLoad {  get; set; }
            public int TemDR { get; set; }
            public int CorrenteSuportadaDR { get; set; }
            public int CorrenteDeProtecaoDR { get; set; }
            public int NumeroDePolosDR { get; set; }
            public ElectricalSystem CircuitObj { get; set; }
            public int TemNeutro { get; set; }
            public int TemTerra { get; set; }

            public string circuitNumber { get; set; }

            public string Name { get; set; }

            public ElementId typeId { get; set; }

            public ElementId levelId { get; set; }

            public int apparentload { get; set; }

            public int voltage { get; set; }

            public int isNotReserveCircuit { get; set; }

            public double length { get; set; }

            public List<Dispositive> dispositives { get; set; }

            public Dictionary<string, Dictionary<string, int>> numOfDispositivesByLoad {  get; set; }

            public string scheme { get; set; }

            public int numOfPoles { get; set; }

            public int numOfNeutrals { get; set; }

            public int numOfGrounds { get; set; }

            private string schemeNumToString (int num)
            {
                return num == 1 ? "" : num.ToString ();
            }

            public string GetScheme ()
            {
                string poles = $"{schemeNumToString(this.numOfPoles)}F";
                string neutrals = !this.numOfNeutrals.Equals(0) ? $" + {schemeNumToString(this.numOfNeutrals)}N" : "";
                string grounds = !this.numOfGrounds.Equals(0) ? $" + {schemeNumToString(this.numOfGrounds)}T" : "";

                string scheme = $"{poles}{neutrals}{grounds}";

                return scheme;
            }

            public Dictionary<string, Dictionary<string, int>> GetNumOfDispositivesByLoad ()
            {
                Dictionary<string, Dictionary<string, int>> numOfDispsByLoad = new Dictionary<string, Dictionary<string, int>>();

                numOfDispsByLoad.Add("Lamps", new Dictionary<string, int>() { { "100", 0 }, { "60", 0 }, { "dif", 0 } });
                numOfDispsByLoad.Add("TUGs-TUEs", new Dictionary<string, int>() { { "TUE", 0 }, { "100", 0 }, { "600", 0 } });

                foreach (Dispositive d in this.dispositives)
                {
                    if (d.dispType == "Lamp")
                    {
                        if (!numOfDispsByLoad["Lamps"].ContainsKey(d.apparentLoad.ToString())) 
                        {
                            numOfDispsByLoad["TUGs-TUEs"]["TUE"] += d.apparentLoad;
                            continue;
                        }

                        numOfDispsByLoad["Lamps"][d.apparentLoad.ToString()] += 1;
                        continue;
                    }

                    if (d.dispType == "TUE")
                    {
                        numOfDispsByLoad["TUGs-TUEs"]["TUE"] += d.apparentLoad;

                        continue;
                    }

                    numOfDispsByLoad["TUGs-TUEs"][d.apparentLoad.ToString()] += 1;
                }

                return numOfDispsByLoad;
            }

        }

        public class Dispositive
        {
            public Dispositive (Element disp, Document doc)
            {
                Utils u = new Utils(doc);

                this.dispositiveElement = disp;

                this.dispositiveInstance = disp as FamilyInstance;

                Dictionary<string, Parameter> dispParams = u.ParamMapToDictonary(disp.ParametersMap);

                this.categoryName = disp.Category.Name;

                if (categoryName != "Dispositivos de iluminação")
                {

                    this.apparentLoad = u.loadStringToInt(dispParams["Potência Aparente (VA)"].AsValueString());
                    this.voltage = u.voltageStringToInt(dispParams["Tensão (V)"].AsValueString());

                }

                this.levelId = disp.LevelId;

                this.typeId = disp.GetTypeId();

                ElectricalSystem dispES = this.dispositiveInstance.MEPModel.GetElectricalSystems().First();

                this.EScircuit = dispES;

                this.circuitName = dispES.LoadName;

                this.connectorManager = this.dispositiveInstance.MEPModel.ConnectorManager;

                this.room = this.dispositiveInstance.Room;

                this.name = dispES.Name;

                this.location = this.dispositiveElement.Location as LocationPoint;

                this.dispType = getDispType();

                this.UsedConnectors = u.GetDispositiveUsedConnectors(this);


            }

            public string getDispType()
            {
                // Checks if this dispositive is a TUE or TUG
                if (this.categoryName == "Luminárias")
                {
                    return "Lamp";
                }

                return this.apparentLoad == 100 || this.apparentLoad == 600 ? "TUG" : "TUE";
            }
            public ElectricalSystem EScircuit { get; set; }
            
            public ElectricalEquipment Panel  { get; set; }

            public string dispType { get; set; }

            public FamilyInstance dispositiveInstance { get; set; }

            public Element dispositiveElement { get; set; }

            public string circuitName { get; set; }

            public int apparentLoad { get; set; }

            public int voltage { get; set; }

            public string categoryName { get; set; }

            public string name { get; set; }

            public ElementId levelId { get; set; }

            public ElementId typeId { get; set; }

            public Room room { get; set; }

            public LocationPoint location { get; set; }

            public ConnectorManager connectorManager { get; set; }

            public List<Connector> UsedConnectors { get; set; }

        }

        public class ConduitAndDispositives 
        {
            public ConduitAndDispositives(List<Conduit> conduits, List<Dispositive> dispositives, List<ElementId> mappedConduits) 
            {
                this.ConduitsList = conduits;
                this.DispositivesList = dispositives;
                this.MappedConduitsIdList = mappedConduits;
            }

            public List<Conduit> ConduitsList { get; set; }

            public List <Dispositive> DispositivesList { get; set; }

            public List<ElementId> MappedConduitsIdList { get; set; }
        }

    }
}
