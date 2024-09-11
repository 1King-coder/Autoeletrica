using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace PlayingWithMEP
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

                this.AssignedCircuits = getCircuits(panelEE, doc);

                this.totalLoad = getPanelTotalLoad(this.AssignedCircuits);

                this.demandedLoad = 0;

                this.Name = panelElement.Name;

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

            public FamilyInstance panelElement { get; set; }

            public ElectricalEquipment PanelObj { get; set; }

            public List<Circuit> AssignedCircuits { get; set; }

            public int totalLoad { get; set; }

            public double demandedLoad { get; set; }

            public string Name { get; set; }

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

            public ElectricalSystem CircuitObj { get; set; }

            public string circuitNumber { get; set; }

            public string Name { get; set; }

            public ElementId typeId { get; set; }

            public ElementId levelId { get; set; }

            public int apparentload { get; set; }

            public int voltage { get; set; }

            public double length { get; set; }

            public List<Dispositive> dispositives { get; set; }

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

                ElectricalSystem dispES = this.dispositiveInstance.MEPModel.GetElectricalSystems().ElementAt(0);

                this.circuitName = dispES.LoadName;

                this.connectorManager = dispES.ConnectorManager;

                this.room = this.dispositiveInstance.Room;

                this.name = dispES.Name;

                this.location = dispES.Location;

            }

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

            public Location location { get; set; }

            public ConnectorManager connectorManager { get; set; }

        }
    }
}
