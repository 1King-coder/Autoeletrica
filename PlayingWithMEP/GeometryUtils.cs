using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayingWithMEP
{
    internal class GeometryUtils
    {
        private Document doc;

        public GeometryUtils(Document doc) {
            this.doc = doc;
        }

        public isParallel (XYZ vec1, XYZ vec2)
        {
            return vec1.DotProduct(vec2);
        }
    }
}
