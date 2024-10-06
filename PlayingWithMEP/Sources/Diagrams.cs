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
using ECs = PlayingWithMEP.ElectricalClasses;

namespace PlayingWithMEP
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

        public FamilySymbol GetElectricalUtilityFamilySymbol (string scheme)
        {
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Diagrama Medidor Concessionária Unifilar", scheme);
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
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Diagrama identificação de circuitos trifilar monofásico", "monofásico");
        }

        public FamilySymbol GetThreeLineBiCircuitIdentifierFamilySymbol()
        {
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Diagrama identificação de circuitos trifilar bifásico", "bifásico");
        }

        public FamilySymbol GetThreeLineTriCircuitIdentifierFamilySymbol()
        {
            return GetGenericAnnotationFamilySymbolByFamilyNameAndTypeName("Diagrama identificação de circuitos trifilar trifásico", "trifásico");
        }

        public List<XYZ> GetDitribuitedCircuitsIdentifiersPosList (int numOfCircuits)
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

            posY.ForEach(x => result.Add(new XYZ(3.45/ 0.3048, x, 0)));

            return result;
        }

    }

}

    

