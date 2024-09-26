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
    internal class ProjectAutomations
    {
        private Document doc;
        private Utils ut;
        private GeometryUtils gUt;
        private List<XYZ> alreadyIdentifiedPoints;

        public ProjectAutomations (Document doc)
        {
            this.doc = doc;
            this.ut = new Utils(doc);
            this.gUt = new GeometryUtils(doc);
            this.alreadyIdentifiedPoints = new List<XYZ>();

        }

        public class IdentifyCircuitsMethod : ProjectAutomations 
        {
            private Dictionary<ElementId, XYZ> alreadyIdentifiedConduits = new Dictionary<ElementId, XYZ>();
            private Dictionary<ElementId, int> alreadyIdentifiedConduitsCount = new Dictionary<ElementId, int>();
            private Dictionary<ElementId, List<string>> alreadyIdentifiedCircuitsInConduit = new Dictionary<ElementId, List<string>>();

            public IdentifyCircuitsMethod(Document doc) : base(doc) { }

            public void IdentifyDispositiveCircuit(ECs.Dispositive dispositive, Reference DispositiveRef)
            {

                ElementId TagId = ut.GetDispositiveCircuitShemeSymbolId(dispositive);

                List<Connector> usedCons = gUt.GetDispositiveUsedConnectors(dispositive);

                Dictionary<int, List<ElementId>> cPathToNextDispositives = gUt.GetConduitsPathsFromDispositive(dispositive);

                List<string> connectedElements = new List<string>();

                foreach (Connector conId in usedCons)
                {
                    List<Conduit> cPath = ut.GetConduitsFromPath(cPathToNextDispositives[conId.Id]);

                    connectedElements.Add(this.doc.GetElement(gUt.GetNextDispositiveFromPath(cPath)).Name);
                }

                //List<Conduit> conduitsPath = ut.GetConduitsFromPath(cPathToNextDispositives[usedCons.Last().Id]);

               // Element nextDisp = this.doc.GetElement(gUt.GetNextDispositiveFromPath(conduitsPath));

                //List<Conduit> condsToPanel = gUt.TrackConduitTilPanel(condAndDisp.DispositivesList, dispositive, condAndDisp.MappedConduitsIdList);

                //List <Conduit> conduitsTilNextDispositive = condAndDisp.ConduitsList;
                Transaction trans = new Transaction(doc);

                trans.Start("Identify Circuit");

                /*foreach (Conduit conduit in conduitsTilNextDispositive)
                {
                    Line line = gUt.GetLineFromConduit(conduit);

                    XYZ conduitMiddlePoint = (line.GetEndPoint(0) + line.GetEndPoint(1)) / 2;


                    XYZ tagPt = new XYZ();
                    XYZ elbowPt = new XYZ();

                    if (gUt.isParallelToY(line.Direction))
                    {
                        tagPt = conduitMiddlePoint + new XYZ(1, 1.5, 0);
                        elbowPt = tagPt - new XYZ(0.5, 0.435, 0);
                    }

                    if (gUt.isParallelToX(line.Direction))
                    {
                        tagPt = conduitMiddlePoint + new XYZ(1.5, 1, 0);
                        elbowPt = tagPt - new XYZ(0.435, 0.435, 0);
                    }

                    if (!gUt.isParallelToX(line.Direction) && !gUt.isParallelToY(line.Direction))
                    {
                        tagPt = conduitMiddlePoint + new XYZ(1, 1, 0);
                        elbowPt = tagPt - new XYZ(0.435, 0.435, 0);
                    }

                    if (this.alreadyIdentifiedCircuitsInConduit.ContainsKey(conduit.Id) && this.alreadyIdentifiedCircuitsInConduit[conduit.Id].Contains(dispositive.EScircuit.Name))
                    {
                        continue;
                    }

                    IndependentTag tag = IndependentTag.Create(this.doc, TagId, this.doc.ActiveView.Id, DispositiveRef, true, TagOrientation.Horizontal, tagPt);
                    tag.LeaderEndCondition = LeaderEndCondition.Free;
                    tag.SetLeaderEnd(DispositiveRef, conduitMiddlePoint);
                    tag.SetLeaderElbow(DispositiveRef, elbowPt);

                    if (this.alreadyIdentifiedConduits.ContainsKey(conduit.Id))
                    {
                        tag.HasLeader = false;
                        tagPt += new XYZ(0.8 * this.alreadyIdentifiedConduitsCount[conduit.Id], 0, 0);
                        this.alreadyIdentifiedConduitsCount[conduit.Id]++;
                    } else
                    {
                        this.alreadyIdentifiedConduits.Add(conduit.Id, tagPt);
                        
                        this.alreadyIdentifiedConduitsCount.Add(conduit.Id, 1);
                    }

                    tag.TagHeadPosition = tagPt;

                    if (!this.alreadyIdentifiedCircuitsInConduit.ContainsKey(conduit.Id))
                    {
                        this.alreadyIdentifiedCircuitsInConduit.Add(conduit.Id, new List<string>());
                        this.alreadyIdentifiedCircuitsInConduit[conduit.Id].Add(dispositive.EScircuit.Name);
                    }


                }

                trans.Commit();*/
            }

            public void identifyAllCircuitsFromPanel(ECs.Panel panel)
            {
                foreach (ECs.Circuit circuit in panel.AssignedCircuits)
                {
                    foreach (ECs.Dispositive dispositive in circuit.dispositives)
                    {
                        Reference dispositiveRef = new Reference(dispositive.dispositiveElement);
                        IdentifyDispositiveCircuit(dispositive, dispositiveRef);

                    }
                }
            }
        }


        
    }
}
