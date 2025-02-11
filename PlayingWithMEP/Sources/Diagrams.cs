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
using ECs = AutoEletrica.ElectricalClasses;

namespace AutoEletrica
{
    internal class Diagrams
    {
        
        private Document doc;

        public Diagrams(Document doc) 
        {
            this.doc = doc;
            
        }

        private FamilySymbol GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName(string familyName, string typeName)
        {
            List<FamilySymbol> fsyms = new FilteredElementCollector(this.doc)
                .OfCategory(BuiltInCategory.OST_GenericAnnotation)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>().ToList();

            FamilySymbol symbolFsym = null;

            fsyms.ForEach(sym => { if (sym.Family.Name == familyName && sym.Name == typeName) { symbolFsym = sym; } });

            return symbolFsym;
        }

        public FamilySymbol GetThreeLineDiagramBodySymbol (string type)
        {
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Diagrama Trifilar - Corpo", type);
        }

        public FamilySymbol GetElectricalUtilityFamilySymbol (string scheme)
        {
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Diagrama Medidor Concessionária Unifilar", scheme);
        }

        public FamilySymbol GetBreakerFamilySymbol (int numOfPoles)
        {
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Disjuntor Unifilar", $"{numOfPoles} F");
        }

        public FamilySymbol GetElectricalEquipmentFamilySymbol(string scheme)
        {
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Diagrama Entrada QDC", scheme);
        }

        public FamilySymbol GetSingleLineCircuitIdentifierFamilySymbol(string scheme)
        {
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Diagrama identificação de circuitos", scheme);
        }

        public FamilySymbol GetThreeLineMonoCircuitIdentifierFamilySymbol()
        {
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Diagrama identificação de circuitos trifilar monofásico", "monofasico");
        }

        public FamilySymbol GetThreeLineBiCircuitIdentifierFamilySymbol()
        {
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Diagrama identificação de circuitos trifilar bifásico", "bifasico");
        }

        public FamilySymbol GetThreeLineTriCircuitIdentifierFamilySymbol()
        {
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Diagrama identificação de circuitos trifilar trifásico", "trifasico");
        }

        public List<XYZ> GetDitribuitedCircuitsIdentifiersPosList (int numOfCircuits, XYZ insertionPt)
        {
            List<XYZ> result = new List<XYZ>();
            List<double> posY = new List<double>();

            int sum = 0;



            for (int i = 0; i < numOfCircuits; i++)
            {

                posY.Add(Math.Pow(-1, i - 1) * 0.898 * sum / 0.3048);

                if (i % 2 == 0) { sum++; }
            }

            posY.Sort();
            posY.Reverse();

            posY.ForEach(y => result.Add(new XYZ((3.45/ 0.3048) + insertionPt.X, y + insertionPt.Y, 0)));

            return result;
        }

        public List<XYZ> GetThreeLineDitribuitedCircuitsIdentifiersPosList(int numOfCircuits)
        {
            List<XYZ> result = new List<XYZ>();
            List<double> posY = new List<double>();

            for (int i = 0; i < numOfCircuits; i++)
            {

                posY.Add(0.76 * i / 0.3048);

            }

            posY.ForEach(y => result.Add(new XYZ(0, y, 0)));


            return result;
        }

    }

}

    

