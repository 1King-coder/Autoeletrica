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


        public Dictionary<int, List<ElementId>> GetConduitsPathsFromDispositive (ECs.Dispositive dispositive, List<ElementId> mappedConduitsId = null)
        {
            if (mappedConduitsId == null) { mappedConduitsId = new List<ElementId>(); }

            List<Connector> consInUse = GetDispositiveUsedConnectors(dispositive);

            Dictionary<int, List<ElementId>> result = new Dictionary<int, List<ElementId>>();

            
            foreach (Connector con in consInUse)
            {
                result.Add(con.Id, new List<ElementId>());

                foreach (Connector nextCon in con.AllRefs)
                {
                    if (nextCon.Owner != null && nextCon.Owner.Category.Name == "Conduites" && !mappedConduitsId.Contains(nextCon.Owner.Id))
                    {
                        mappedConduitsId.Add(nextCon.Owner.Id);
                        result[con.Id] = mapConduitCons(nextCon.Owner as MEPCurve, mappedConduitsId.ConvertAll(elId => elId));
                    }
                }
                mappedConduitsId.Clear();
            }

            return result;
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

        public List<Element> GetConnectedElectricalElements (ECs.Dispositive dispositive)
        {
            List<Connector> usedCons = GetDispositiveUsedConnectors(dispositive);

            Dictionary<int, List<ElementId>> cPathToNextDispositives = GetConduitsPathsFromDispositive(dispositive);

            List<Element> connectedElements = new List<Element>();

            foreach (Connector conId in usedCons)
            {
                List<Conduit> cPath = ut.GetConduitsFromPath(cPathToNextDispositives[conId.Id]);

                connectedElements.Add(this.doc.GetElement(GetNextDispositiveFromPath(cPath)));
            }

            return connectedElements;
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
