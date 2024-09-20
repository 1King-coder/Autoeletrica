using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Electrical;
using ECs = PlayingWithMEP.ElectricalClasses;
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
        private Utils ut;

        public GeometryUtils(Document doc) {
            this.doc = doc;
            this.ut = new Utils(doc);
        }

        public Line GetLineFromConduit (Conduit conduit)
        {
            return ConvertCurveToLine((conduit.Location as LocationCurve).Curve);
        }

        public List<Connector> GetDispositiveUsedConnectors (ECs.Dispositive dispositive)
        {
            ConnectorSet unusedCons = dispositive.connectorManager.UnusedConnectors;
            ConnectorSet allCons = dispositive.connectorManager.Connectors;

            List<int> unusedConsId = new List<int>();

            foreach (Connector c in unusedCons)
            {
                unusedConsId.Add(c.Id);
            }

            List<Connector>  usedCons = new List<Connector>();

            foreach (Connector con in allCons)
            {
                if (!unusedConsId.Contains(con.Id))
                {
                    usedCons.Add(con);
                }
            }

            return usedCons;
            
        }

        public List<Connector> GetConduitUsedConnectors (Conduit conduit)
        {
            ConnectorSet unusedCons = conduit.ConnectorManager.UnusedConnectors;
            ConnectorSet allCons = conduit.ConnectorManager.Connectors;

            List<int> unusedConsId = new List<int>();

            foreach (Connector c in unusedCons)
            {
                unusedConsId.Add(c.Id);
            }

            List<Connector> usedCons = new List<Connector>();

            foreach (Connector con in allCons)
            {
                if (!unusedConsId.Contains(con.Id))
                {
                    usedCons.Add(con);
                }
            }

            return usedCons;

        }

        public List<ElementId> mapConduitCons (MEPCurve conduit, List<ElementId> mappedConduitsId)
        {
            ConnectorSet ConSet = conduit.ConnectorManager.Connectors;
            

            foreach (Connector c in ConSet)
            {
                foreach (Connector nextCon in c.AllRefs)
                {
                    if (nextCon.Owner.Category.Name == "Conexões do conduite" &&  !mappedConduitsId.Contains(nextCon.Owner.Id))
                    {
                        mappedConduitsId.Add(nextCon.Owner.Id);
                        mapConduitElbowsCons(nextCon.Owner as FamilyInstance, mappedConduitsId);
                    }

                    if (nextCon.Owner.Category.Name == "Conduites" && !mappedConduitsId.Contains(nextCon.Owner.Id))
                    {
                        mappedConduitsId.Add(nextCon.Owner.Id);
                    }

                }
            }

            return mappedConduitsId;
        }



        public List<ElementId> mapConduitElbowsCons (FamilyInstance elbow, List<ElementId> mappedConduitsId)
        {
            
            ConnectorSet ConSet = elbow.MEPModel.ConnectorManager.Connectors;

            foreach (Connector c in ConSet)
            {
                foreach (Connector nextCon in c.AllRefs)
                {
                    if (!mappedConduitsId.Contains(nextCon.Owner.Id))
                    {
                        mappedConduitsId.Add(nextCon.Owner.Id);
                        mapConduitCons(nextCon.Owner as MEPCurve, mappedConduitsId);

                    }
                }
            }
            return mappedConduitsId;
        }

        public bool isValidParallelConduit (Conduit conduit)
        {
            Line conduitLine = GetLineFromConduit(conduit);

            return isParallelToXAndY(conduitLine.Direction) && isBigEnough(conduitLine);
            
        }

        public List<Conduit> TrackConduitsToNextES (List<Connector> connectors, List<ElementId> mappedConduitsId = null, List<Conduit> parallelConduits = null)
        {
            if (mappedConduitsId == null) { mappedConduitsId = new List<ElementId>(); }
            if (parallelConduits == null) { parallelConduits = new List<Conduit>(); }
            List<Conduit> cs = new List<Conduit>();

            foreach (Connector con in connectors)
            {
                foreach (Connector nextCon in con.AllRefs)
                {

                    if (nextCon.Owner.Category.Name == "Conduites")
                    {
                        
                        mappedConduitsId = mapConduitCons(nextCon.Owner as MEPCurve, mappedConduitsId);
                        
                         
                        foreach (ElementId elId in mappedConduitsId)
                        {
                            Element conduitEl = this.doc.GetElement(elId);

                            if (conduitEl.Category.Name == "Conduites")
                            {
                                if (isValidParallelConduit(conduitEl as Conduit))
                                {
                                    parallelConduits.Add(conduitEl as Conduit);
                                }
                            }
                        }
                    }
                }
            }

            return parallelConduits;
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
