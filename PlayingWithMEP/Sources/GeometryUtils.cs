using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Electrical;
using ECs = AutoEletrica.ElectricalClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEletrica
{
    internal class GeometryUtils
    {
        private Document doc;
        private Utils ut;

        public GeometryUtils(Document doc) {
            this.doc = doc;
            this.ut = new Utils(doc);
        }

        public Line GetLineFromConduit (Conduit conduit)
        {
            return ConvertCurveToLine((conduit.Location as LocationCurve).Curve);
        }

        public bool isValidParallelConduit (Conduit conduit)
        {
            Line conduitLine = GetLineFromConduit(conduit);

            return isParallelToXAndY(conduitLine.Direction) && isBigEnough(conduitLine);
            
        }


        public ElementId GetNextDispositiveFromPath (List<Conduit> cPath)
        {
            foreach (Connector con in cPath.Last().ConnectorManager.Connectors)
            {
                if (con != null)
                {
                    foreach(Connector nextCon in con.AllRefs)
                    {
                        if (ut.isDispositive(nextCon.Owner) || ut.isLuminary(nextCon.Owner) || ut.isElectricEquipment(nextCon.Owner))
                        {
                            return nextCon.Owner.Id;
                        }
                    }
                }
            }

            return null;

        }

        public List<Conduit> GetParallelToXYConduits (List<Conduit> conduits)
        {
            List<Conduit> parallelToXYConduits = new List<Conduit>();

            foreach (Conduit C in conduits)
            {
                Line cLine = GetLineFromConduit(C);

                if (isParallelToXAndY(cLine.Direction) && isBigEnough(GetLineFromConduit(C)))
                {
                    parallelToXYConduits.Add(C);
                }
            }

            return parallelToXYConduits;
        }
        
        public Dictionary<int, List<Conduit>> GetParallelToXYConduitsFromPath (Dictionary<int, List<ElementId>> PathsFromConsObject)
        {
            Dictionary<int, List<Conduit>> result = new Dictionary<int, List<Conduit>>();

            foreach (int conId in PathsFromConsObject.Keys)
            {
                List<Conduit> conduitsPath = ut.GetConduitsFromPath(PathsFromConsObject[conId]);
                result.Add(conId, GetParallelToXYConduits(conduitsPath));
            }

            return result;
        }

        public bool isParallelToXAndY(XYZ vec)
        {
            return Math.Round(vec.DotProduct(new XYZ(0, 0, 1)), 2) == 0;
        }

        public bool isParallelToY (XYZ vec)
        {
            return Math.Round(vec.DotProduct(new XYZ(0, 1, 0)), 2) == 0;
        }

        public bool isParallelToX(XYZ vec)
        {
            return Math.Round(vec.DotProduct(new XYZ(1, 0, 0)), 2) == 0;
        }

        public XYZ VectorFromTwoPoints (XYZ p1, XYZ p2)
        {
            return p1 - p2;
        }

        public bool isBigEnough (Line conduitLine)
        {
            return Math.Round(conduitLine.Length / 3.281, 2) > 0.5;
        }

        public Line ConvertCurveToLine (Curve c)
        {
            return Line.CreateBound(c.GetEndPoint(0), c.GetEndPoint(1));
        }

        

    }
}
