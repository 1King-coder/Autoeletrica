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
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace PlayingWithMEP
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

        public Dictionary<int, Dictionary<ElementId, List<ElementId>>> GetPathsToNextDispositiveOrTElbowFromDispositive(ECs.Dispositive dispositive)
        {
            List<ElementId> mappedElements = new List<ElementId>();
            Dictionary<int, Dictionary<ElementId, List<ElementId>>> result = new Dictionary<int, Dictionary<ElementId, List<ElementId>>>();



            foreach (Connector con in dispositive.UsedConnectors)
            {
                result.Add(con.Id, new Dictionary<ElementId, List<ElementId>>());
                
                Connector nextCon = ut.GetConnectorListFromConnectorSet(con.AllRefs).Last();

                
                List<ElementId> pathToDispositive = new List<ElementId>();
                while (!ut.isElectricalComponent(nextCon.Owner) && !ut.isElbowConnectedToMultipleConduits((nextCon.Owner as Conduit)))
                {
                    Element nextEl = ut.GetConnectorListFromConnectorSet(nextCon.ConnectorManager.Connectors).Last().Owner;

                    // verificar codigo


                    if (nextEl != null)
                    {

                        pathToDispositive.Add(nextEl.Id);
                        List<Connector> nextElConnectors = ut.GetConnectorListFromConnectorSet((nextEl as Conduit).ConnectorManager.Connectors);
                        nextCon = nextElConnectors.Last();
                        
                    }

                }

                result[con.Id].Add(nextCon.Owner.Id, pathToDispositive.ConvertAll(el => el));

                pathToDispositive.Clear();

            }



            return result;

        }

        public List<ElementId> SimpleElbowForwardMapping (Conduit simpleElbow, List<ElementId> mappedElements = null)
        {
            if (mappedElements == null) { mappedElements = new List<ElementId>(); }

            List<Connector> usedConnectors = ut.GetConduitElbowUsedConnectors(simpleElbow);
            List<ElementId> result = new List<ElementId>();

            Connector nextConduit = usedConnectors.Last();

            return result;
            
        }


        public Dictionary<int, List<ElementId>> GetPathsToNextDispositiveOrTElbowFromPanel(ECs.Panel panel)
        {
            List<ElementId> mappedElements = new List<ElementId>();
            Dictionary<int, List<ElementId>> result = new Dictionary<int, List<ElementId>>();



            foreach (Connector con in panel.UsedConnectors)
            {

            }



            return result;
        }
    }
}
