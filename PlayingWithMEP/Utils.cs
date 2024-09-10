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

            pickedRef = sel.PickObject(ObjectType.Element, "Select the Eletric panel");

            if (pickedRef == null)
            {
                return null;
            }

            Element selectedPanel = this.doc.GetElement(pickedRef);

            return selectedPanel as FamilyInstance;
        }

        public Panel getSelectedPanel (FamilyInstance panelElement)
        {
            Panel newPanel = new Panel();

            ElectricalEquipment panelEE = panelElement.MEPModel as ElectricalEquipment;

            newPanel.panelElement = panelElement;

            newPanel.PanelObj = panelEE;

            newPanel.AssignedCircuits = getCircuits(panelEE);

            newPanel.totalLoad = getPanelTotalLoad(newPanel.AssignedCircuits);

            newPanel.demandedLoad = 0;

            newPanel.Name = panelElement.Name;

            return newPanel;

        }

        public int getPanelTotalLoad (List<Circuit> panelCircuits)
        {
            int totalLoad = 0;
            foreach (Circuit circuit in panelCircuits)
            {
                totalLoad += circuit.apparentload;
            }

            return totalLoad;
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

        public Circuit electricalSystemToCircuit (ElectricalSystem ES)
        {
            Circuit newCircuit = new Circuit();

            newCircuit.CircuitObj = ES;

            newCircuit.circuitNumber = ES.CircuitNumber;

            newCircuit.Name = ES.LoadName;

            string apparentLoadString = ES.get_Parameter(BuiltInParameter.RBS_ELEC_APPARENT_LOAD).AsValueString();

            newCircuit.apparentload = loadStringToInt(apparentLoadString);

            newCircuit.length = getLongerPath(ES);

            newCircuit.typeId = ES.GetTypeId();

            newCircuit.levelId = ES.LevelId;

            return newCircuit;
        }

        public double getLongerPath (ElectricalSystem ES)
        {
            Transaction changeTrans1 = new Transaction(doc);
            Transaction changeTrans2 = new Transaction(doc);

            changeTrans1.Start("Changing Circuit " + ES.CircuitNumber + " path mode to AllDevices");
            ES.CircuitPathMode = ElectricalCircuitPathMode.AllDevices;
            double length1 = Math.Round(ES.Length / 3.281, 2);
            changeTrans1.Commit();

            changeTrans2.Start("Changing Circuit " + ES.CircuitNumber + " path mode to FarthestDevice");
            ES.CircuitPathMode = ElectricalCircuitPathMode.FarthestDevice;
            double lenght2 = Math.Round(ES.Length / 3.281, 2);
            changeTrans2.Commit();

            
            return Math.Max(length1, lenght2);
        }

        public List<Circuit> getCircuits (ElectricalEquipment panel)
        {
            List<Circuit> circuitList = new List<Circuit>();

            foreach (ElectricalSystem eS in panel.GetAssignedElectricalSystems())
            {
                Circuit c = electricalSystemToCircuit(eS);

                circuitList.Add(c);
            }

            return circuitList;

        }

        public Dispositive DispositiveFromElement (Element el)
        {
            Dispositive newDispositive = new Dispositive();

            newDispositive.dispositiveElement = el;

            newDispositive.dispositiveInstance = el as FamilyInstance;

            


        }
    }
    public class Panel 
    {
        public FamilyInstance panelElement { get; set; }
 
        public ElectricalEquipment PanelObj {  get; set; }

        public List<Circuit> AssignedCircuits { get; set; }

        public int totalLoad {  get; set; }

        public double demandedLoad { get; set; }

        public string Name { get; set; }


    }

    public class Circuit
    {
        public ElectricalSystem CircuitObj { get; set; }
        
        public string circuitNumber { get; set; }

        public string Name { get; set; }

        public ElementId typeId { get; set; }

        public ElementId levelId { get; set; }

        public int apparentload { get; set; }

        public double length { get; set; }

        public List<Dispositive> dispositives { get; set; }

    }

    public class Dispositive
    {
        public FamilyInstance dispositiveInstance { get; set; }

        public Element dispositiveElement { get; set; }

        public Circuit circuit { get; set; }

        public string apparentLoad { get; set; }

        public string categoryName { get; set; }

        public string name { get; set; }

        public string dispositiveType { get; set; }

        public ElementId LevelId { get; set; }

        public ElementId typeId { get; set; }

        public ElementId typeId { get; set; }

    }
}
