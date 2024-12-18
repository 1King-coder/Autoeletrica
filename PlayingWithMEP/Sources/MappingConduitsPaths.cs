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
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace AutoEletrica
{
    internal class MappingConduitsPaths
    {
        private Document doc;
        private Utils ut;
        private GeometryUtils gUt;
        public Dictionary<ElementId, Dictionary<int, List<ElementId>>> PathsToElementTree;

        /*
         * The tree structure is:
         *  PathTree = {
         *      ElementId1: {
         *          1: Path1,
         *          2: Path2,
         *          ...
         *      },
         *      ElementId2: {
         *          1: Path1,
         *          2: Path2,
         *          ...
         *      },
         *      ...
         *  }
         */

        public MappingConduitsPaths(Document doc)
        {
            this.doc = doc;
            this.ut = new Utils(doc);
            this.gUt = new GeometryUtils(doc);
        }



        
    }
}
