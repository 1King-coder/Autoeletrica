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
    }
    
}
