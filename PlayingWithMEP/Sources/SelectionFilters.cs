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

    internal class SelectionFilterDispositives : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element == null) { return false; }

            if (!(element is FamilyInstance)) { return false; }

            List<BuiltInCategory> categories = new List<BuiltInCategory>() {
                BuiltInCategory.OST_ElectricalFixtures,
                BuiltInCategory.OST_LightingFixtures,
                BuiltInCategory.OST_LightingDevices,
                BuiltInCategory.OST_ElectricalEquipment,

            };

            if (!categories.Contains(element.Category.BuiltInCategory)) { return false; }

            Regex er1 = new Regex(@"^(?!1\s)^\d+\s+Tomadas\s+.+$", RegexOptions.None);
            Regex er2 = new Regex(@"^(?!1\s)^\d+\s+Teclas\s+.+$", RegexOptions.None);
            Regex er3 = new Regex(@"^\d+\s+.+?\s+(?:e|\+)\s+\d+\s+.+?(?:\s+.+)?$", RegexOptions.None);

            if (er1.IsMatch(element.Name)) { return false; }
            if (er2.IsMatch(element.Name)) { return false; }
            if (er3.IsMatch(element.Name)) { return false; }

            return true;
        }

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
