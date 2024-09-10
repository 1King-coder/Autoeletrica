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

        public List<ElectricalSystem> ESSetToList (ISet<ElectricalSystem> iset)
        {
            List<ElectricalSystem> newList = new List<ElectricalSystem>();

            foreach (ElectricalSystem el in iset)
            {
                newList.Add (el);
            }

            return newList;
        }

        public List<BuiltInParameter> circuitParams (ParameterSet paramSet)
        {

            List<BuiltInParameter> parametersNames = new List<BuiltInParameter> ();

            parametersNames.Add(BuiltInParameter.RBS_ELEC_APPARENT_LOAD);
            parametersNames.Add(BuiltInParameter.RBS_ELEC_CIRCUIT_NAME);
            parametersNames.Add(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER);
            parametersNames.Add(BuiltInParameter.RBS_ELEC_CIRCUIT_LENGTH_PARAM);
            parametersNames.Add(BuiltInParameter.RBS_ELEC_VOLTAGE);
            parametersNames.Add(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_HOTS_PARAM);
            parametersNames.Add(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_NEUTRALS_PARAM);
            parametersNames.Add(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_GROUNDS_PARAM);

            return parametersNames;
        }

        public Circuit electricalSystemToCircuit (ElectricalSystem ES)
        {
            Circuit newCircuit = new Circuit();

            newCircuit.CircuitObj = ES;

            newCircuit.Name = ES.Name;

            

            


            return newCircuit;
        }

        public void getElectricalSystems (FamilyInstance panel)
        {

        }
    }
    public class Panel 
    {
        public ElectricalEquipment PanelObj {  get; set; }

        public List<Circuit> AssignedCircuits { get; set; }


    }

    public class Circuit
    {
        public ElectricalSystem CircuitObj { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public double apparentload { get; set; }
    }


}
