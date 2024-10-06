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
using PlayingWithMEP.Sources;
using System.Runtime.CompilerServices;
using System.Threading;


namespace PlayingWithMEP
{
    internal class ProjectAutomations
    {
        private Document doc;
        private Utils ut;
        private GeometryUtils gUt;
        private PlanilhaDimensionamentoEletrico planilha;
        public MappingConduitsPaths mapping;
        private List<XYZ> alreadyIdentifiedPoints;

        public ProjectAutomations (Document doc, PlanilhaDimensionamentoEletrico planilha)
        {
            this.doc = doc;
            this.ut = new Utils(doc);
            this.gUt = new GeometryUtils(doc);
            this.planilha = planilha;
            this.alreadyIdentifiedPoints = new List<XYZ>();
            this.mapping = new MappingConduitsPaths(doc);
            this.planilha = planilha;
        }

        public class IdentifyCircuitsClass : ProjectAutomations 
        {
            private Dictionary<ElementId, XYZ> alreadyIdentifiedConduits = new Dictionary<ElementId, XYZ>();
            private Dictionary<ElementId, int> alreadyIdentifiedConduitsCount = new Dictionary<ElementId, int>();
            private Dictionary<ElementId, List<string>> alreadyIdentifiedCircuitsInConduit = new Dictionary<ElementId, List<string>>();

            public IdentifyCircuitsClass(Document doc, PlanilhaDimensionamentoEletrico planilha) : base(doc, planilha) { }

            public void IdentifyDispositiveCircuit(ECs.Dispositive dispositive, Reference DispositiveRef)
            {

                Dictionary<int, Dictionary<ElementId, List<ElementId>>> paths = mapping.GetPathsToNextDispositiveOrTElbowFromDispositive(dispositive);

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

        
        public class GenerateDiagramsClass : ProjectAutomations 
        {

            public Diagrams diagrams;
            public Dictionary<string, string> breakers { get; set; }
            public Dictionary<string, string> cableSeccions {  get; set; }
            public Dictionary<string, Dictionary<string, string>> circuitsLoadsPerPhase { get; set; }
            private View singleLineView;
            private View threeLineView;



            public GenerateDiagramsClass(Document doc, PlanilhaDimensionamentoEletrico planilha) : base(doc, planilha) 
            {
                this.diagrams = new Diagrams(doc);
                this.singleLineView = this.ut.GetViewByName("Diagrama Unifilar");
                this.threeLineView = this.ut.GetViewByName("Diagrama Trifilar");
            }

            public void GetCircuitsInfosFromSpreadsheet (ECs.Panel panel)
            {
                this.breakers = this.planilha.GetAllCircuitsBreakersAmps(panel);
                this.cableSeccions = this.planilha.GetAllCircuitsCableSeccion(panel);
                this.circuitsLoadsPerPhase = this.planilha.GetCircuitsLoadPerPhase(panel);
            }

            public FamilyInstance GenElectricalUtilitySymbol (ElectricalUtilityData elecUdata, FamilySymbol fmsym)
            {

                Transaction trans = new Transaction(this.doc);
                trans.Start("Generating Single-line Electrical utility Symbol Diagram");

                FamilyInstance ElecU = this.doc.Create.NewFamilyInstance(new XYZ(), fmsym, this.singleLineView);


                ElecU.LookupParameter("Corrente do disjuntor").Set(elecUdata.CorrenteDisjuntor);
                ElecU.LookupParameter("Secção dos cabos").SetValueString(elecUdata.SeccaoCabos);

                trans.Commit();

                return ElecU;
            }

            public FamilyInstance GenSingleLineCircuitIdentifierSymbol (CircuitsIdentifierData circuitsIdentifierData, XYZ insertionPt, FamilySymbol fmsym) 
            {

                Transaction trans = new Transaction(this.doc);

                trans.Start("Generating Single-line circuits Symbol Diagram");

                FamilyInstance circIden = this.doc.Create.NewFamilyInstance(insertionPt, fmsym, this.singleLineView);
                this.doc.Regenerate();
                circIden.LookupParameter("Corrente do Disjuntor").Set(circuitsIdentifierData.CorrenteDisjuntor);
                circIden.LookupParameter("Descrição Circuito").Set(circuitsIdentifierData.Descricao);
                circIden.LookupParameter("Fase Circuito").Set(circuitsIdentifierData.GetPhasesWithLoad());
                circIden.LookupParameter("Não Reserva").Set(circuitsIdentifierData.NaoReserva);
                circIden.LookupParameter("Secção dos cabos").SetValueString(circuitsIdentifierData.SeccaoCabos);
                circIden.LookupParameter("Potência Circuito").Set(circuitsIdentifierData.Potencia);

                trans.Commit();

                return circIden;
            }

            public FamilyInstance GenPanelSymbol (PanelIdentifierData panelIdentifierData, FamilySymbol fmsym)
            {
                Transaction trans = new Transaction(this.doc);

                trans.Start("Generating Single-line panel Symbol Diagram");

                FamilyInstance panelIden = this.doc.Create.NewFamilyInstance(new XYZ(), fmsym, this.singleLineView);
                this.doc.Regenerate();

                panelIden.LookupParameter("Corrente do disjuntor").Set(panelIdentifierData.CorrenteDisjuntorGeral);
                panelIden.LookupParameter("Secção dos Cabos").SetValueString(panelIdentifierData.SeccaoCabos);
                panelIden.LookupParameter("Número de polos DR").Set(panelIdentifierData.NumeroPolosDR);
                panelIden.LookupParameter("Corrente DR").Set(panelIdentifierData.CorrenteDR);
                panelIden.LookupParameter("Corrente de proteção DDR (mA)").Set(panelIdentifierData.CorrenteProtecaoDR);
                panelIden.LookupParameter("Corrente nominal DPS (kA)").Set(panelIdentifierData.CorrenteNominalDPS);
                panelIden.LookupParameter("Tensão nominal DPS").Set(panelIdentifierData.TensaoNominalDPS);
                panelIden.LookupParameter("Classe de proteção DPS").SetValueString(panelIdentifierData.ClasseDeProtecaoDPS);
                panelIden.LookupParameter("DPS para o Neutro").Set(panelIdentifierData.DPSneutro);

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

                return circuitsIdentifierData;
            }

            public ElectricalUtilityData SetUpElecetricalUtilityData (int correnteDisjuntor, string seccaoCabos)
            {
                ElectricalUtilityData elecUData = new ElectricalUtilityData();

                elecUData.CorrenteDisjuntor = correnteDisjuntor;
                elecUData.SeccaoCabos = seccaoCabos;

                return elecUData;
            }
            
            public PanelIdentifierData SetUpPanelIdentifierData (
                int CorrenteDisjuntorGeral,
                string ClasseProtecaoDPS,
                int CorrenteNominalDPS,
                int TensaoNominalDPS,
                int CorrenteDR,
                int CorrenteProtecaoDR,
                int NumeroPolosDR,
                string SeccaoCabos,
                int DPSneutro
               )
            {
                PanelIdentifierData panelIdentifierData = new PanelIdentifierData();

                panelIdentifierData.CorrenteDisjuntorGeral = CorrenteDisjuntorGeral;
                panelIdentifierData.DPSneutro = DPSneutro;
                panelIdentifierData.TensaoNominalDPS = TensaoNominalDPS;
                panelIdentifierData.ClasseDeProtecaoDPS = ClasseProtecaoDPS;
                panelIdentifierData.CorrenteNominalDPS = CorrenteNominalDPS;
                panelIdentifierData.CorrenteDR = CorrenteDR;
                panelIdentifierData.CorrenteProtecaoDR = CorrenteProtecaoDR;
                panelIdentifierData.NumeroPolosDR = NumeroPolosDR;
                panelIdentifierData.SeccaoCabos = SeccaoCabos;

                return panelIdentifierData; 
            }

            public void GenSingleLineDiagramFromPanel (ECs.Panel panel)
            {
                
                this.GetCircuitsInfosFromSpreadsheet(panel);


                FamilySymbol fsymEU = this.diagrams.GetElectricalUtilityFamilySymbol(this.ut.GetShemeToDiagrams("3F + N + T"));
                FamilySymbol fsymPanel = this.diagrams.GetElectricalEquipmentFamilySymbol(this.ut.GetShemeToDiagrams(panel.scheme));

                ElectricalUtilityData elecUData = SetUpElecetricalUtilityData(32, "4,0");

                GenElectricalUtilitySymbol(elecUData, fsymEU);

                PanelIdentifierData panelIData = SetUpPanelIdentifierData(
                    32,
                    "III",
                    30,
                    175,
                    63,
                    30,
                    4,
                    "6,0",
                    0
                    );

                GenPanelSymbol(panelIData, fsymPanel);

                List<XYZ> distribuitedInsertionPoints = this.diagrams.GetDitribuitedCircuitsIdentifiersPosList(panel.AssignedCircuits.Count);

                int counter = 0;


                foreach (ECs.Circuit circuit in panel.AssignedCircuits)
                {
                    CircuitsIdentifierData circuitIData = SetUpCircuitData(circuit);
                    FamilySymbol fsymCircuit = this.diagrams.GetSingleLineCircuitIdentifierFamilySymbol(this.ut.GetShemeToDiagrams(circuit.scheme));

                    GenSingleLineCircuitIdentifierSymbol(circuitIData, distribuitedInsertionPoints[counter], fsymCircuit);

                    counter++;
                }


            }

        }


    }
}
