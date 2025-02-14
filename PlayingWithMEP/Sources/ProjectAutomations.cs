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
using AutoEletrica.Sources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Security.Cryptography;
using Autodesk.Revit.Exceptions;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Reflection;
using System.Windows.Ink;
using System.Collections.ObjectModel;


namespace AutoEletrica
{
    internal class ProjectAutomations
    {
        private Document doc;
        private Utils ut;
        private GeometryUtils gUt;
        public MappingConduitsPaths mapping;
        private List<XYZ> alreadyIdentifiedPoints;

        public ProjectAutomations (Document doc)
        {
            this.doc = doc;
            this.ut = new Utils(doc);
            this.gUt = new GeometryUtils(doc);
            this.alreadyIdentifiedPoints = new List<XYZ>();
            this.mapping = new MappingConduitsPaths(doc);
        }

        public class IdentifyCircuitsClass : ProjectAutomations 
        {
            private Dictionary<ElementId, XYZ> alreadyIdentifiedConduits = new Dictionary<ElementId, XYZ>();
            private Dictionary<ElementId, int> alreadyIdentifiedConduitsCount = new Dictionary<ElementId, int>();
            private Dictionary<ElementId, List<string>> alreadyIdentifiedCircuitsInConduit = new Dictionary<ElementId, List<string>>();

            public IdentifyCircuitsClass(Document doc) : base(doc) { }

            public void IdentifyDispositiveCircuit(ECs.Dispositive dispositive, Reference DispositiveRef)
            {

                //Dictionary<int, Dictionary<ElementId, List<ElementId>>> paths = mapping.GetPathsToNextDispositiveOrTElbowFromDispositive(dispositive);

                //Transaction trans = new Transaction(doc);

                //trans.Start("Identify Circuit");

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

            public void indentifyAllDispositivesFromPanel (ECs.Panel panel)
            {
                Transaction trans = new Transaction(doc);
                trans.Start($"Identifying all dispostives from {panel.Name}");

                foreach (ECs.Circuit circ in  panel.AssignedCircuits)
                {
                    foreach (ECs.Dispositive dispositive in circ.dispositives)
                    {
                        FamilySymbol fsym = ut.SymbolIdForPowerDispositives();

                        if (dispositive.dispType == "Lamp")
                        {
                            fsym = ut.symbolIdForIluminationDispositivesOnRoof();

                            if (dispositive.dispositiveElement.Name.Contains("Arandela"))
                            {
                                fsym = ut.symbolIdForIluminationDispositivesOnWall();
                            }

                            if (dispositive.dispositiveElement.Category.Name == "Dispositivos de iluminação")
                            {
                                fsym = ut.SymbolIdForSwitches();
                            }
                        }

                        XYZ tagPt = dispositive.location.Point + (dispositive.dispType == "Lamp" ? new XYZ() : dispositive.dispositiveInstance.FacingOrientation.Multiply( 0.5 * 0.3048) + new XYZ(0, - 0.1*0.3048, 0));
                        Reference dispRef = new Reference(dispositive.dispositiveElement);

                        IndependentTag tag = IndependentTag.Create(this.doc, fsym.Id, this.doc.ActiveView.Id, dispRef, false, TagOrientation.Horizontal, tagPt);
                        
                    }
                }

                trans.Commit();
            }

            public void identifyDispositiveCircuitScheme (ECs.Dispositive dispositive, XYZ leaderEndPt = null, XYZ leaderElbowPt = null, bool leaderAdded = false)
            {
                
                ElementId tagId = ut.GetDispositiveCircuitShemeSymbolId(dispositive);

                if (dispositive.dispositiveElement.Name.Contains("Interruptor"))
                {
                    tagId = ut.SymbolIdForSwitchesScheme().Id;
                }

                Transaction transaction = new Transaction(doc);

                transaction.Start("Identifying dispositive");

                XYZ tagPt = leaderElbowPt;

                if (leaderEndPt == null)
                {
                    tagPt = dispositive.location.Point + (dispositive.dispType == "Lamp" ? new XYZ() : dispositive.dispositiveInstance.FacingOrientation.Multiply(ut.metersToFeet(0.6)) - new XYZ(0, ut.metersToFeet(0.1), 0));
                    leaderAdded = true;
                }

                if (leaderElbowPt == null && leaderEndPt != null)
                {
                    leaderElbowPt = leaderEndPt.Add(new XYZ(ut.metersToFeet(0.6), ut.metersToFeet(0.6), 0));
                }

                
                tagPt = tagPt.Add(new XYZ (ut.metersToFeet(0.2), ut.metersToFeet(0.135),0));
   
                Reference dispRef = new Reference(dispositive.dispositiveElement);

                IndependentTag tag = IndependentTag.Create(this.doc, tagId, this.doc.ActiveView.Id, dispRef, !leaderAdded, TagOrientation.Horizontal, tagPt);

                if (!leaderAdded && leaderEndPt != null && leaderElbowPt != null)
                {
                    tag.LeaderEndCondition = LeaderEndCondition.Free;
                    tag.SetLeaderEnd(dispRef, leaderEndPt);
                    tag.SetLeaderElbow(dispRef, leaderElbowPt);
                    tag.TagHeadPosition = tagPt;
                }
                


                transaction.Commit();
            }

            public void identifyMultipleDispositiveCircuitScheme (List<ECs.Dispositive> dispositives, XYZ leaderEndPt, XYZ leaderElbowPt)
            {
                int counter = 0;
                XYZ pt = null;
                List<string> alreadyIdentified = new List<string>();

                bool leaderAdded = false;

                foreach (ECs.Dispositive dispositive in dispositives)
                {
                    if (leaderElbowPt != null) { pt = leaderElbowPt + new XYZ(0.8 * counter, 0, 0); }

                    if (!alreadyIdentified.Contains(dispositive.dispositiveElement.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsValueString()))
                    {
                        identifyDispositiveCircuitScheme(dispositive, leaderEndPt, pt, leaderAdded);
                        counter++;
                        alreadyIdentified.Add(dispositive.dispositiveElement.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER).AsValueString());
                        leaderAdded = true;
                    }
                    

                }
            }

        }

        
        public class GenerateDiagramsClass : ProjectAutomations 
        {

            public Diagrams diagrams;
            public Dictionary<string, string> breakers { get; set; }
            public Dictionary<string, string> cableSeccions {  get; set; }
            public Dictionary<string, Dictionary<string, string>> circuitsLoadsPerPhase { get; set; }
            public View singleLineView;
            public View threeLineView;
            private PlanilhaDimensionamentoEletrico planilha;


            public GenerateDiagramsClass(Document doc, PlanilhaDimensionamentoEletrico planilha) : base(doc) 
            {
                this.diagrams = new Diagrams(doc);
                this.singleLineView = this.ut.GetViewByName("Diagrama Unifilar");
                this.threeLineView = this.ut.GetViewByName("Diagrama Trifilar");
                this.planilha = planilha;
            }

            public void GetCircuitsInfosFromSpreadsheet (ECs.Panel panel)
            {
                this.breakers = this.planilha.GetAllCircuitsBreakersAmps(panel);
                this.cableSeccions = this.planilha.GetAllCircuitsCableSeccion(panel);
                this.circuitsLoadsPerPhase = this.planilha.GetCircuitsLoadPerPhase(panel);
            }

            public FamilyInstance GenElectricalUtilitySymbol (ElectricalUtilityData elecUdata, FamilySymbol fmsym, XYZ insertionPt)
            {

                Transaction trans = new Transaction(this.doc);
                trans.Start("Generating Single-line Electrical utility Symbol Diagram");

                FamilyInstance ElecU = this.doc.Create.NewFamilyInstance(insertionPt, fmsym, this.singleLineView);


                ElecU.LookupParameter("Corrente do disjuntor").Set(elecUdata.CorrenteDisjuntor);
                ElecU.LookupParameter("Secção dos cabos").Set(elecUdata.SeccaoCabos);

                trans.Commit();

                return ElecU;
            }

            public FamilyInstance GenSingleLineCircuitIdentifierSymbol (CircuitsIdentifierData circuitsIdentifierData, XYZ insertionPt, FamilySymbol fmsym) 
            {

                Transaction trans = new Transaction(this.doc);

                trans.Start("Generating Single-line circuits Symbol Diagram");

                FamilyInstance circIden = this.doc.Create.NewFamilyInstance(insertionPt, fmsym, this.singleLineView);
                circIden.LookupParameter("Corrente do Disjuntor").Set(circuitsIdentifierData.CorrenteDisjuntor);
                circIden.LookupParameter("Descrição Circuito").Set($"{circuitsIdentifierData.NumeroCircuito} - {circuitsIdentifierData.Descricao}");
                circIden.LookupParameter("Fase Circuito").Set(circuitsIdentifierData.GetPhasesWithLoad());
                circIden.LookupParameter("Não Reserva").Set(circuitsIdentifierData.NaoReserva);
                circIden.LookupParameter("Secção dos cabos").SetValueString(circuitsIdentifierData.SeccaoCabos);
                circIden.LookupParameter("Potência Circuito").Set(circuitsIdentifierData.Potencia);
                circIden.LookupParameter("Número Circuito").Set(circuitsIdentifierData.NumeroCircuito);
                circIden.LookupParameter("Tensão").Set(circuitsIdentifierData.Tensao);
                circIden.LookupParameter("Frequência").Set(circuitsIdentifierData.Frequencia);

                trans.Commit();

                return circIden;
            }

            public void CreateThreeLineDiagramBody (ThreeLinePanelIdenfierData panelIdenData )
            {
                double distanceBetweenBreakers = ut.metersToFeet(0.6);

                double diagramHeight = ut.metersToFeet(3.5) + distanceBetweenBreakers * panelIdenData.numOfCircuits + ut.metersToFeet(1);

                Line spaceLine = Line.CreateBound(new XYZ(), new XYZ(0, ut.metersToFeet(0.4), 0));
                Line line1 = Line.CreateBound(new XYZ(), new XYZ(ut.metersToFeet(-2), ut.metersToFeet(-3.4641), 0));
                Line line2 = Line.CreateBound(new XYZ(), new XYZ(0, ut.metersToFeet(-3.4641), 0));
                Line line3 = Line.CreateBound(new XYZ(), new XYZ(ut.metersToFeet(2), ut.metersToFeet(-3.4641), 0));

                Line PhaseALine = Line.CreateBound(line1.GetEndPoint(1), new XYZ (line1.GetEndPoint(1).X, -diagramHeight, 0));
                Line PhaseBLine = Line.CreateBound(line2.GetEndPoint(1), new XYZ (line2.GetEndPoint(1).X, -diagramHeight, 0));
                Line PhaseCLine = Line.CreateBound(line3.GetEndPoint(1), new XYZ (line3.GetEndPoint(1).X, -diagramHeight, 0));

                Line NeutralLine1 = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(ut.metersToFeet(10), 0, 0));
                Line NeutralLine2 = Line.CreateBound(new XYZ(ut.metersToFeet(10), 0, 0), new XYZ(ut.metersToFeet(10), 0, 0).Subtract(new XYZ(0, diagramHeight, 0)));

                Line GroundLine1 = Line.CreateBound(new XYZ(ut.metersToFeet(-2), ut.metersToFeet(0.8), 0), new XYZ(ut.metersToFeet(-2), 0, 0));
                Line GroundLine2 = Line.CreateBound(new XYZ(ut.metersToFeet(-2), 0, 0), new XYZ(ut.metersToFeet(-10), 0, 0)); 

                Line GroundLine3 = Line.CreateBound(new XYZ(ut.metersToFeet(-10), 0, 0), new XYZ(ut.metersToFeet(-10), 0, 0).Subtract(new XYZ(0, diagramHeight, 0)));

                FamilySymbol breakerSym = diagrams.GetBreakerFamilySymbol(panelIdenData.numOfPoles);

                Transaction trans = new Transaction(doc);
                trans.Start("Instanciate three line diagram body");
                XYZ breakerPt = new XYZ(0, ut.metersToFeet(0.4), 0);
                FamilyInstance mainBreaker = this.doc.Create.NewFamilyInstance(breakerPt, breakerSym, this.threeLineView);
                mainBreaker.LookupParameter("Corrente").Set(panelIdenData.CorrenteDisjuntor);

                Line basisZaxis = Line.CreateUnbound(breakerPt, XYZ.BasisZ);

                ElementTransformUtils.RotateElement(doc, mainBreaker.Id, basisZaxis, Math.PI / 2);

                this.doc.Create.NewDetailCurve(this.threeLineView, spaceLine);
                this.doc.Create.NewDetailCurve(this.threeLineView, line1);
                this.doc.Create.NewDetailCurve(this.threeLineView, line2);
                this.doc.Create.NewDetailCurve(this.threeLineView, line3);
                this.doc.Create.NewDetailCurve(this.threeLineView, PhaseALine);
                this.doc.Create.NewDetailCurve(this.threeLineView, PhaseBLine);
                this.doc.Create.NewDetailCurve(this.threeLineView, PhaseCLine);
                this.doc.Create.NewDetailCurve(this.threeLineView, NeutralLine1);
                this.doc.Create.NewDetailCurve(this.threeLineView, NeutralLine2);
                this.doc.Create.NewDetailCurve(this.threeLineView, GroundLine1);
                this.doc.Create.NewDetailCurve(this.threeLineView, GroundLine2);
                this.doc.Create.NewDetailCurve(this.threeLineView, GroundLine3);

                TextNote.Create(doc, this.threeLineView.Id, new XYZ(ut.metersToFeet(0.3), ut.metersToFeet(0.3), 0), $"F #{panelIdenData.SeccaoCabos} mm²", this.doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));
                TextNote.Create(doc, this.threeLineView.Id, NeutralLine1.GetEndPoint(1).Subtract(new XYZ(ut.metersToFeet(0.6),-ut.metersToFeet(0.3),0)), $"N #{panelIdenData.SeccaoCabos} mm²", this.doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));
                TextNote.Create(doc, this.threeLineView.Id, GroundLine2.GetEndPoint(1).Add(new XYZ(0, ut.metersToFeet(0.3), 0)), $"T #{panelIdenData.SeccaoCabos} mm²", this.doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));
                TextNote.Create(doc, this.threeLineView.Id, PhaseALine.GetEndPoint(1).Add(new XYZ(ut.metersToFeet(0.2), ut.metersToFeet(0.2), 0)), $"A", this.doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));
                TextNote.Create(doc, this.threeLineView.Id, PhaseBLine.GetEndPoint(1).Add(new XYZ(ut.metersToFeet(0.2), ut.metersToFeet(0.2), 0)), $"B", this.doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));
                TextNote.Create(doc, this.threeLineView.Id, PhaseCLine.GetEndPoint(1).Add(new XYZ(ut.metersToFeet(0.2), ut.metersToFeet(0.2), 0)), $"C", this.doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));

                ut.DrawRectangle(this.threeLineView, new XYZ(GroundLine2.GetEndPoint(1).X - ut.metersToFeet(0.8), GroundLine2.GetEndPoint(1).Y + ut.metersToFeet(2), 0), new XYZ(NeutralLine2.GetEndPoint(1).X + ut.metersToFeet(0.8), NeutralLine2.GetEndPoint(1).Y - ut.metersToFeet(0.8), 0));

                TextNote.Create(doc, this.threeLineView.Id, new XYZ(GroundLine2.GetEndPoint(1).X - ut.metersToFeet(0.3), GroundLine2.GetEndPoint(1).Y + ut.metersToFeet(1.7), 0), panelIdenData.name, this.doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));

                trans.Commit();
            }

            public FamilyInstance GenThreeLineCircuitIdentifierSymbol(ThreeLineCircuitsIdentifierData circuitsIdentifierData, XYZ insertionPt, FamilySymbol fmsym)
            {

                Transaction trans = new Transaction(this.doc);

                trans.Start("Generating Three-line circuits Symbol Diagram");

                FamilyInstance circIden = this.doc.Create.NewFamilyInstance(insertionPt, fmsym, this.threeLineView);

                if (circuitsIdentifierData.numOfPoles == 3)
                {
                    SetTriCircIdentifierElement(circIden, circuitsIdentifierData, insertionPt);

                    trans.Commit();

                    mirrorThreeLineCircuitIdentifierIfHasPhaseA(circuitsIdentifierData, circIden);

                    return circIden;
                }

                if (circuitsIdentifierData.numOfPoles == 2)
                {
                    SetBiCircIdentifierElement(circIden, circuitsIdentifierData, insertionPt);
                    trans.Commit();

                    mirrorThreeLineCircuitIdentifierIfHasPhaseA(circuitsIdentifierData, circIden);

                    return circIden;
                }

                SetMonoCircIdentifierElement(circIden, circuitsIdentifierData, insertionPt);

                trans.Commit();

                mirrorThreeLineCircuitIdentifierIfHasPhaseA(circuitsIdentifierData, circIden);

                Transaction t = new Transaction(this.doc);
                
                return circIden;
            }

            public void mirrorThreeLineCircuitIdentifierIfHasPhaseA (ThreeLineCircuitsIdentifierData circuitsIdentifierData, FamilyInstance circIden)
            {
                
                if (circuitsIdentifierData.GetPhasesWithLoad().Contains("A"))
                {
                    Transaction trans = new Transaction(this.doc);

                    trans.Start("Mirroring Element");
                    ElementTransformUtils.MirrorElement(doc, circIden.Id, Plane.CreateByNormalAndOrigin(XYZ.BasisX, new XYZ()));
                    this.doc.Delete(circIden.Id);
                    trans.Commit();
                }
            }


            public void SetMonoCircIdentifierElement (FamilyInstance circIden, ThreeLineCircuitsIdentifierData circuitsIdentifierData, XYZ insertionPt)
            {

                circIden.LookupParameter("Tensão").Set(circuitsIdentifierData.Tensao);
                circIden.LookupParameter("Frequência").Set(circuitsIdentifierData.Frequencia);
                circIden.LookupParameter("Corrente do Disjuntor").Set(circuitsIdentifierData.CorrenteDisjuntor);
                circIden.LookupParameter("Descrição Circuito").Set($"{circuitsIdentifierData.NumeroCircuito} - {circuitsIdentifierData.Descricao}");
                circIden.LookupParameter("Fase Circuito").Set(circuitsIdentifierData.GetPhasesWithLoad());
                circIden.LookupParameter("Não Reserva").Set(circuitsIdentifierData.NaoReserva);
                if (!circuitsIdentifierData.NaoReserva.Equals(0))
                {
                    circIden.LookupParameter("Potência Circuito").Set(Convert.ToInt32(circuitsIdentifierData.circuitLoadPerPhase[circuitsIdentifierData.GetPhasesWithLoad()]));
                }
                else
                {
                    circIden.LookupParameter("Potência Circuito").Set(circuitsIdentifierData.totalLoad);

                }

                XYZ circlePt = insertionPt;

                if (circuitsIdentifierData.GetPhasesWithLoad().Contains("B"))
                {
                    Line lineB = Line.CreateBound(insertionPt, insertionPt.Add(new XYZ(-2 / 0.3048, 0, 0)));
                    this.doc.Create.NewDetailCurve(threeLineView, lineB);
                    circlePt = insertionPt.Add(new XYZ(-2 / 0.3048, 0, 0));
                }
                if (circuitsIdentifierData.GetPhasesWithLoad().Contains("A"))
                {
                    circlePt = insertionPt.Subtract(new XYZ (ut.metersToFeet(4), 0, 0));
                }


                ut.CreateCircularFilledRegion(doc, this.threeLineView, ut.metersToFeet(0.1), circlePt);
            }

            public void SetBiCircIdentifierElement (FamilyInstance circIden, ThreeLineCircuitsIdentifierData circuitsIdentifierData, XYZ insertionPt)
            {
                string[] fases = circuitsIdentifierData.GetPhasesWithLoad().Split(',');

                string fase1 = "";
                string fase2 = "";

                if (!circuitsIdentifierData.Descricao.Contains("Reserva"))
                {
                    fase1 = fases[0].Trim();
                    fase2 = fases[1].Trim();

                }

                circIden.LookupParameter("Tensão").Set(circuitsIdentifierData.Tensao);
                circIden.LookupParameter("Frequência").Set(circuitsIdentifierData.Frequencia);

                circIden.LookupParameter("Corrente do Disjuntor").Set(circuitsIdentifierData.CorrenteDisjuntor);
                circIden.LookupParameter("Descrição Circuito").Set($"{circuitsIdentifierData.NumeroCircuito} - {circuitsIdentifierData.Descricao}");
                circIden.LookupParameter("Fase 1 Circuito").Set(fase1);
                circIden.LookupParameter("Fase 2 Circuito").Set(fase2);
                circIden.LookupParameter("Não Reserva").Set(circuitsIdentifierData.NaoReserva);
                if (circuitsIdentifierData.Descricao.Contains("Reserva"))
                {
                    circIden.LookupParameter("Potência Circuito Fase 1").Set(Convert.ToInt32(circuitsIdentifierData.totalLoad / 2));
                    circIden.LookupParameter("Potência Circuito Fase 2").Set(Convert.ToInt32(circuitsIdentifierData.totalLoad / 2));
                }
                else
                {
                    circIden.LookupParameter("Potência Circuito Fase 1").Set(Convert.ToInt32(circuitsIdentifierData.circuitLoadPerPhase[fase1]));
                    circIden.LookupParameter("Potência Circuito Fase 2").Set(Convert.ToInt32(circuitsIdentifierData.circuitLoadPerPhase[fase2]));
                }

                XYZ circlePt = insertionPt;

                Line line = Line.CreateBound(insertionPt, insertionPt.Add(new XYZ(-2 / 0.3048, 0, 0)));

                if (circuitsIdentifierData.GetPhasesWithLoad().Contains("A"))
                {
                    circlePt = insertionPt.Add(new XYZ(ut.metersToFeet(-4), ut.metersToFeet(0.754), 0));
                    ut.CreateCircularFilledRegion(doc, this.threeLineView, ut.metersToFeet(0.1), circlePt);

                    if (circuitsIdentifierData.GetPhasesWithLoad().Contains("B"))
                    {

                        line = Line.CreateBound(insertionPt.Subtract(new XYZ(ut.metersToFeet(4), 0, 0)), insertionPt.Add(new XYZ(-2 / 0.3048, 0, 0)));
                        ut.CreateCircularFilledRegion(doc, this.threeLineView, ut.metersToFeet(0.1), line.GetEndPoint(1));
                    } else
                    {
                        line = Line.CreateBound(insertionPt.Subtract(new XYZ(ut.metersToFeet(4), 0, 0)), insertionPt);
                        ut.CreateCircularFilledRegion(doc, this.threeLineView, ut.metersToFeet(0.1), insertionPt);

                    }
                }

                if (circuitsIdentifierData.GetPhasesWithLoad().Contains("B") && circuitsIdentifierData.GetPhasesWithLoad().Contains("C"))
                {
                    circlePt = insertionPt.Add(new XYZ(ut.metersToFeet(-2), ut.metersToFeet(0.754), 0));

                    line = Line.CreateBound(insertionPt.Add(new XYZ(0, ut.metersToFeet(0.754), 0)), insertionPt.Add(new XYZ(ut.metersToFeet(-2), ut.metersToFeet(0.754), 0))); ;
                    ut.CreateCircularFilledRegion(doc, this.threeLineView, ut.metersToFeet(0.1), circlePt);
                    ut.CreateCircularFilledRegion(doc, this.threeLineView, ut.metersToFeet(0.1), insertionPt);

                }


                this.doc.Create.NewDetailCurve(threeLineView, line);
            }

            public void SetTriCircIdentifierElement(FamilyInstance circIden, ThreeLineCircuitsIdentifierData circuitsIdentifierData, XYZ insertionPt)
            {
                string[] fases = circuitsIdentifierData.GetPhasesWithLoad().Split(',');

                string fase1 = "";
                string fase2 = "";
                string fase3 = "";

                if (!circuitsIdentifierData.Descricao.Contains("Reserva"))
                {
                    fase1 = fases[0].Trim();
                    fase2 = fases[1].Trim();
                    fase3 = fases[2].Trim();

                }
                circIden.LookupParameter("Tensão").Set(circuitsIdentifierData.Tensao);
                circIden.LookupParameter("Frequência").Set(circuitsIdentifierData.Frequencia);
                circIden.LookupParameter("Corrente do Disjuntor").Set(50);
                circIden.LookupParameter("Descrição Circuito").Set($"{circuitsIdentifierData.NumeroCircuito} - {circuitsIdentifierData.Descricao}");
                circIden.LookupParameter("Fase 1 Circuito").Set(fase1);
                circIden.LookupParameter("Fase 2 Circuito").Set(fase2);
                circIden.LookupParameter("Fase 3 Circuito").Set(fase3);
                circIden.LookupParameter("Não Reserva").Set(circuitsIdentifierData.NaoReserva);
                if (circuitsIdentifierData.Descricao.Contains("Reserva"))
                {
                    circIden.LookupParameter("Potência Circuito Fase 1").Set(Convert.ToInt32(circuitsIdentifierData.totalLoad / 3));
                    circIden.LookupParameter("Potência Circuito Fase 2").Set(Convert.ToInt32(circuitsIdentifierData.totalLoad / 3));
                    circIden.LookupParameter("Potência Circuito Fase 3").Set(Convert.ToInt32(circuitsIdentifierData.totalLoad / 3));
                }
                else
                {
                    circIden.LookupParameter("Potência Circuito Fase 1").Set(Convert.ToInt32(circuitsIdentifierData.circuitLoadPerPhase[fase1]));
                    circIden.LookupParameter("Potência Circuito Fase 2").Set(Convert.ToInt32(circuitsIdentifierData.circuitLoadPerPhase[fase2]));
                    circIden.LookupParameter("Potência Circuito Fase 3").Set(Convert.ToInt32(circuitsIdentifierData.circuitLoadPerPhase[fase3]));
                }

                ut.CreateCircularFilledRegion(doc, this.threeLineView, ut.metersToFeet(0.1), insertionPt.Add(new XYZ(ut.metersToFeet(-4), ut.metersToFeet(0.754 * 2), 0)));

                Line lineB = Line.CreateBound(insertionPt.Subtract(new XYZ(ut.metersToFeet(4), ut.metersToFeet(-0.752), 0)), insertionPt.Subtract(new XYZ(ut.metersToFeet(2), ut.metersToFeet(-0.752), 0)));
                ut.CreateCircularFilledRegion(doc, this.threeLineView, ut.metersToFeet(0.1), lineB.GetEndPoint(1));

                Line lineC = Line.CreateBound(insertionPt, insertionPt.Subtract(new XYZ(ut.metersToFeet(4), 0, 0)));
                ut.CreateCircularFilledRegion(doc, this.threeLineView, ut.metersToFeet(0.1), lineC.GetEndPoint(0));

                this.doc.Create.NewDetailCurve(threeLineView, lineB);
                this.doc.Create.NewDetailCurve(threeLineView, lineC);
            }

            public FamilyInstance GenPanelSymbol (PanelIdentifierData panelIdentifierData, FamilySymbol fmsym, XYZ insertionPt)
            {
                Transaction trans = new Transaction(this.doc);

                trans.Start("Generating Single-line panel Symbol Diagram");

                FamilyInstance panelIden = this.doc.Create.NewFamilyInstance(insertionPt, fmsym, this.singleLineView);

                panelIden.LookupParameter("Corrente do disjuntor").Set(panelIdentifierData.CorrenteDisjuntorGeral);
                panelIden.LookupParameter("Secção dos Cabos").Set(panelIdentifierData.SeccaoCabos);
                panelIden.LookupParameter("Número de polos DR").Set(panelIdentifierData.NumeroPolosDR);
                panelIden.LookupParameter("Corrente DR").Set(panelIdentifierData.CorrenteDR);
                panelIden.LookupParameter("Corrente de proteção DDR (mA)").Set(panelIdentifierData.CorrenteProtecaoDR);
                panelIden.LookupParameter("Corrente nominal DPS (kA)").Set(panelIdentifierData.CorrenteNominalDPS);
                panelIden.LookupParameter("Tensão nominal DPS").Set(panelIdentifierData.TensaoNominalDPS);
                panelIden.LookupParameter("Classe de proteção DPS").SetValueString(panelIdentifierData.ClasseDeProtecaoDPS);
                panelIden.LookupParameter("DPS para o Neutro").Set(panelIdentifierData.DPSneutro);
                panelIden.LookupParameter("DPS Visível").Set(panelIdentifierData.HasDPS);
                panelIden.LookupParameter("DR Visível").Set(panelIdentifierData.HasGeneralDR);

                trans.Commit();

                return panelIden;
            }

            
            public CircuitsIdentifierData SetUpCircuitData (ECs.Circuit circuit)
            {
                CircuitsIdentifierData circuitsIdentifierData = new CircuitsIdentifierData();

                circuitsIdentifierData.Descricao = circuit.Name;
                circuitsIdentifierData.Potencia = circuit.apparentload;
                circuitsIdentifierData.NumeroCircuito = circuit.circuitNumber;
                circuitsIdentifierData.circuitLoadPerPhase = this.circuitsLoadsPerPhase[circuit.circuitNumber];
                circuitsIdentifierData.SeccaoCabos = this.cableSeccions[circuit.circuitNumber];
                circuitsIdentifierData.NaoReserva = circuit.isNotReserveCircuit;
                circuitsIdentifierData.CorrenteDisjuntor = Convert.ToInt32(this.breakers[circuit.circuitNumber]);
                circuitsIdentifierData.Tensao = Convert.ToInt32(circuit.voltage);
                circuitsIdentifierData.Frequencia = 60;

                return circuitsIdentifierData;
            }

            public ThreeLineCircuitsIdentifierData SetUpThreeLineCircuitData(ECs.Circuit circuit)
            {
                ThreeLineCircuitsIdentifierData circuitsIdentifierData = new ThreeLineCircuitsIdentifierData();

                circuitsIdentifierData.Descricao = circuit.Name;
                circuitsIdentifierData.NumeroCircuito = circuit.circuitNumber;
                circuitsIdentifierData.circuitLoadPerPhase = this.circuitsLoadsPerPhase[circuit.circuitNumber];
                circuitsIdentifierData.NaoReserva = circuit.isNotReserveCircuit;
                circuitsIdentifierData.CorrenteDisjuntor = Convert.ToInt32(this.breakers[circuit.circuitNumber]);
                circuitsIdentifierData.totalLoad = circuit.apparentload;
                circuitsIdentifierData.numOfPoles = circuit.numOfPoles;
                circuitsIdentifierData.Tensao = circuit.voltage;
                circuitsIdentifierData.Frequencia = 60;

                return circuitsIdentifierData;
            }

            public ThreeLinePanelIdenfierData SetUpThreeLinePanelData(ECs.Panel panel, int CorrenteDisjuntor, string SeccaoCabos)
            {
                ThreeLinePanelIdenfierData panelIData = new ThreeLinePanelIdenfierData();

                panelIData.CorrenteDisjuntor = CorrenteDisjuntor;
                panelIData.SeccaoCabos = SeccaoCabos;
                panelIData.numOfCircuits = panel.AssignedCircuits.Count();
                panelIData.numOfPoles = panel.numOfPoles;
                panelIData.name = panel.Name;

                return panelIData;

            }




            public void GenSingleLineDiagramFromPanel (ECs.Panel panel, PanelIdentifierData panelIData, ElectricalUtilityData elecUData, XYZ insertionPt, bool ShowElecU)
            {
                
                this.GetCircuitsInfosFromSpreadsheet(panel);


                FamilySymbol fsymEU = this.diagrams.GetElectricalUtilityFamilySymbol(this.ut.GetShemeToDiagrams("3F + N + T"));
                FamilySymbol fsymPanel = this.diagrams.GetElectricalEquipmentFamilySymbol(this.ut.GetShemeToDiagrams(panel.scheme));

                if (ShowElecU)
                {
                    GenElectricalUtilitySymbol(elecUData, fsymEU, insertionPt);
                }

                GenPanelSymbol(panelIData, fsymPanel, insertionPt);

                List<XYZ> distribuitedInsertionPoints = this.diagrams.GetDitribuitedCircuitsIdentifiersPosList(panel.AssignedCircuits.Count(), insertionPt);

                int counter = 0;


                foreach (ECs.Circuit circuit in panel.AssignedCircuits)
                {
                    CircuitsIdentifierData circuitIData = SetUpCircuitData(circuit);
                    FamilySymbol fsymCircuit = this.diagrams.GetSingleLineCircuitIdentifierFamilySymbol(this.ut.GetShemeToDiagrams(circuit.scheme));

                    GenSingleLineCircuitIdentifierSymbol(circuitIData, distribuitedInsertionPoints[counter], fsymCircuit);

                    counter++;
                }
                Transaction trans = new Transaction(doc);
                trans.Start("Creating single line diagram rectangle");

                ut.DrawRectangle(this.singleLineView, new XYZ(insertionPt.X + ut.metersToFeet(0.2), distribuitedInsertionPoints.First().Y + ut.metersToFeet(2), 0), new XYZ(insertionPt.X + ut.metersToFeet(6), distribuitedInsertionPoints.Last().Y - ut.metersToFeet(0.5), 0));

                trans.Commit();
            }

            public void SetupThreeLineDiagramBody (ThreeLineDiagramBody threeLineDiagramObj)
            {
                List<PropertyInfo> properties = threeLineDiagramObj.GetType().GetProperties().ToList();
                properties.Remove(properties.Find(e => e.Name == "ThreeLineDiagramFI"));
                string debugStr = "";
                
                properties.ForEach(e => { debugStr += $"{((Dictionary<string, string>) e.GetValue(threeLineDiagramObj)).Keys.ToList()[0]} \n"; });

                TaskDialog.Show("Debug", debugStr);

            }

            public void GenThreeLineDiagramFromPanel(ECs.Panel panel, ThreeLineDiagramBody threeLineDiagObj)
            {

                this.GetCircuitsInfosFromSpreadsheet(panel);
                FamilySymbol threelineDiagFS = this.diagrams.GetThreeLineDiagramBodySymbol("Trifasico");

                threeLineDiagObj.NomeDoQD.Add("Nome do QD", panel.Name);
                threeLineDiagObj.QtdeDeCircuitos.Add("Qtde circuitos", panel.AssignedCircuits.Count());


                SetupThreeLineDiagramBody(threeLineDiagObj);

                //threeLineDiagObj.ThreeLineDiagramFI = this.doc.Create.NewFamilyInstance(new XYZ(), threelineDiagFS, this.threeLineView);



                //ThreeLinePanelIdenfierData threeLinePanelIdenfierBody= this.SetUpThreeLinePanelData(panel, CorrenteDisjuntor, SeccaoCabos);

                // this.CreateThreeLineDiagramBody(threeLinePanelIdenfierData);
                /*
                float counter = 0;
                

                foreach (ECs.Circuit circuit in panel.AssignedCircuits)
                {
                    if (circuit.Name.ToLower().Contains("reserva")) { continue; }

                    ThreeLineCircuitsIdentifierData circuitIData = SetUpThreeLineCircuitData(circuit);

                    FamilySymbol fsymCircuit = this.diagrams.GetThreeLineMonoCircuitIdentifierFamilySymbol();

                    if (circuit.numOfPoles == 2)
                    {
                        counter += 1.2f;
                        fsymCircuit = this.diagrams.GetThreeLineBiCircuitIdentifierFamilySymbol();
                    }
                    if (circuit.numOfPoles == 3)
                    {
                        counter += 2.4f;
                        fsymCircuit = this.diagrams.GetThreeLineTriCircuitIdentifierFamilySymbol();
                    }

                    if (circuit.isNotReserveCircuit == 0)
                    {
                        counter--;
                    }

                    XYZ pt = new XYZ(ut.metersToFeet(2), -ut.metersToFeet(0.76 * counter) - ut.metersToFeet(3.8), 0);

                    GenThreeLineCircuitIdentifierSymbol(circuitIData, pt, fsymCircuit);

                    counter += 0.6f;
  
                }
                */


            }

        }


    }
}
