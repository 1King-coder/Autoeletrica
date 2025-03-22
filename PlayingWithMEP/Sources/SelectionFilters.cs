using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEletrica.Sources
{
    
    internal class SelectionFilterNoSelectedDispositives : ISelectionFilter
    {
        private List<ElementId> selectedOnes;
        public SelectionFilterNoSelectedDispositives(List<ElementId> selectedOnes)
        {
            this.selectedOnes = selectedOnes;
        }
        public bool AllowElement(Element element)
        {
            if (!Utils.isValidDispositive(element)) { return false; }

            var circuito = (element as FamilyInstance).MEPModel.GetElectricalSystems();

            if (circuito != null || circuito.Count != 0) { return false; }
            if (selectedOnes.Contains(element.Id)) { return false; }

            return true;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    internal class SelectionFilterNoCircuitDispositives : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (!Utils.isValidDispositive(element)) { return false; }

            var circuito = (element as FamilyInstance).MEPModel.GetElectricalSystems();

            if (circuito != null || circuito.Count != 0) { return false; }

            return true;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    internal class SelectionFilterOnlySelectedDispositives : ISelectionFilter
    {
        private List<ElementId> selectedOnes;
        public SelectionFilterOnlySelectedDispositives(List<ElementId> selectedOnes)
        {
            this.selectedOnes = selectedOnes;
        }
        public bool AllowElement(Element element)
        {
            if (!Utils.isValidDispositive(element)) { return false; }


            var circuito = (element as FamilyInstance).MEPModel.GetElectricalSystems();

            if (circuito != null || circuito.Count != 0) { return false; }
            if (!selectedOnes.Contains(element.Id)) { return false; }

            return true;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    internal class SelectionFilterDispositives : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            return Utils.isValidDispositive(element);
;        }

        public bool AllowReference(Reference reference, XYZ pos)
        {
            return false;
        }

    }

    internal class SelectionFilterPanels : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element == null) { return false; }

            if (!(element is FamilyInstance)) { return false; }

                

            if (element.Category.BuiltInCategory != BuiltInCategory.OST_ElectricalEquipment) { return false; }

            return true;

        }

        public bool AllowReference(Reference reference, XYZ pos)
        {
            return false;
        }
    }

    
}
