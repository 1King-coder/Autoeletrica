﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Electrical;
using ECs = AutoEletrica.ElectricalClasses;
using AutoEletrica.Sources;
using System.Reflection;
using Autodesk.Revit.DB.Architecture;

using static AutoEletrica.ElectricalClasses;


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
                        FamilySymbol fsym = dispositive.apparentLoad == 100 ? ut.SymbolIdForPowerDispositivesBelow100load() : ut.SymbolIdForPowerDispositives();

                        if (dispositive.dispType == "Lamp")
                        {
                            fsym = ut.symbolIdForIluminationDispositivesOnRoof();

                            if (dispositive.dispositiveElement.Name.Contains("Arandela"))
                            {
                                fsym = ut.symbolIdForIluminationDispositivesOnWall();
                            }
                        }

                        if (dispositive.dispositiveElement.Category.Name.ToLower() == "dispositivos de iluminação")
                        {
                            fsym = ut.SymbolIdForSwitches();
                        }

                        XYZ tagPt = dispositive.location.Point + (dispositive.dispType == "Lamp" ? new XYZ() : dispositive.dispositiveInstance.FacingOrientation.Multiply(ut.metersToFeet(0.35)));
                        Reference dispRef = new Reference(dispositive.dispositiveElement);

                        IndependentTag tag = IndependentTag.Create(this.doc, fsym.Id, this.doc.ActiveView.Id, dispRef, false, TagOrientation.Horizontal, tagPt);
                        
                    }
                }

                trans.Commit();
            }

            public void identifyDispositiveCircuitScheme (ECs.Dispositive dispositive, XYZ leaderEndPt = null, XYZ leaderElbowPt = null, bool leaderAdded = false)
            {
                
                ElementId tagId = ut.GetDispositiveCircuitShemeSymbolId(dispositive);

                if (dispositive.dispositiveElement.Category.Name.ToLower() == "dispositivos de iluminação")
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
            public int totalLoad { get; set; }
            public int demandedLoad { get; set; }


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
                this.totalLoad = this.planilha.GetTotalLoadFromPanel();
                this.demandedLoad = this.planilha.GetDemandedLoadFromPanel();
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
                circIden.LookupParameter("DR Visível").Set(circuitsIdentifierData.TemDR);
                circIden.LookupParameter("Corrente de Proteção DR (mA)").Set(circuitsIdentifierData.CorrenteDeProtecaoDR);
                circIden.LookupParameter("Corrente do DR").Set(circuitsIdentifierData.CorrenteDoDR);
                circIden.LookupParameter("Número de polos DR").Set(circuitsIdentifierData.NumeroDePolosDR);

                trans.Commit();

                return circIden;
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
                circuitsIdentifierData.TemDR = circuit.TemDR;
                circuitsIdentifierData.CorrenteDeProtecaoDR = circuit.CorrenteDeProtecaoDR;
                circuitsIdentifierData.CorrenteDoDR = circuit.CorrenteSuportadaDR;
                circuitsIdentifierData.NumeroDePolosDR = circuit.NumeroDePolosDR;

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




            public void GenSingleLineDiagramFromPanel (ECs.Panel panel, PanelIdentifierData panelIData, ElectricalUtilityData elecUData=null, XYZ insertionPt = null, bool ShowElecU=false)
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

                XYZ topLeftCorner = new XYZ(insertionPt.X + ut.metersToFeet(0.2), distribuitedInsertionPoints.First().Y + ut.metersToFeet(2), 0);
                XYZ bottomRightCorner = new XYZ(insertionPt.X + ut.metersToFeet(6), distribuitedInsertionPoints.Last().Y - ut.metersToFeet(0.5), 0);

                ut.DrawRectangle(this.singleLineView, topLeftCorner, bottomRightCorner);

                FamilySymbol panelDataSym = this.diagrams.GetSingleLineDiagramPanelDataSymbol();
                FamilyInstance panelDataInstance = this.doc.Create.NewFamilyInstance(topLeftCorner + new XYZ(ut.metersToFeet(0.1), ut.metersToFeet(0.1), 0), panelDataSym, this.singleLineView);
                panelDataInstance.LookupParameter("Nome do Painel").Set(panel.Name);
                panelDataInstance.LookupParameter("Pot. Instalada (VA)").Set(Convert.ToInt32(this.totalLoad));
                panelDataInstance.LookupParameter("Pot. Demandada (VA)").Set(Convert.ToInt32(this.demandedLoad));
                panelDataInstance.LookupParameter("Tensão").Set(panel.PanelObj.DistributionSystem.Name);
                panelDataInstance.LookupParameter("Frequência").Set(60);

                trans.Commit();
            }

            public void SetupThreeLineDiagramBody (ThreeLineDiagramBody threeLineDiagramObj)
            {
                List<PropertyInfo> properties = threeLineDiagramObj.GetType().GetProperties().ToList();
                properties.Remove(properties.Find(e => e.Name == "ThreeLineDiagramFI"));
                
                properties.ForEach(e =>
                {
                    var value = e.GetValue(threeLineDiagramObj);
                    
                    if (value is Dictionary<string, int> intDict)
                    {
                        if (intDict.Keys.Count() != 0)
                        {
                            string paramName = intDict.Keys.ToList()[0];
                            threeLineDiagramObj.ThreeLineDiagramFI.LookupParameter(paramName).Set(intDict[paramName]);
                        }
                    }
                    else if (value is Dictionary<string, string> stringDict)
                    {
                        if (stringDict.Keys.Count() != 0)
                        {
                            string paramName = stringDict.Keys.ToList()[0];
                            threeLineDiagramObj.ThreeLineDiagramFI.LookupParameter(paramName).Set(stringDict[paramName]);
                        }
                    }
                    else if (value is Dictionary<string, bool> boolDict)
                    {
                        if (boolDict.Keys.Count() != 0)
                        {
                            string paramName = boolDict.Keys.ToList()[0];
                            threeLineDiagramObj.ThreeLineDiagramFI.LookupParameter(paramName).Set(boolDict[paramName] ? 1 : 0);
                        }
                    }
                });
            }

            public void SetupThreeLineDiagramCircuitIdentifier (ThreeLineDiagramCircuitIdentifier threeLineDiagCircuitIden)
            {
                List<PropertyInfo> properties = threeLineDiagCircuitIden.GetType().GetProperties().ToList();
                properties.Remove(properties.Find(e => e.Name == "CircuitIdentifierFI"));
                foreach (PropertyInfo e in properties) 
                {
                    var value = e.GetValue(threeLineDiagCircuitIden);
                    if (value is Dictionary<string, int> intDict)
                    {
                        if (intDict.Keys.Count() != 0)
                        {
                            string paramName = intDict.Keys.ToList()[0];
                            threeLineDiagCircuitIden.CircuitIdentifierFI.LookupParameter(paramName).Set(intDict[paramName]);
                        }
                    }
                    else if (value is Dictionary<string, string> stringDict)
                    {
                        if (stringDict.Keys.Count() != 0)
                        {
                            string paramName = stringDict.Keys.ToList()[0];
                            threeLineDiagCircuitIden.CircuitIdentifierFI.LookupParameter(paramName).Set(stringDict[paramName]);
                        }
                    }
                    else if (value is Dictionary<string, bool> boolDict)
                    {
                        if (e.Name == "Conexoes")
                        {
                            boolDict.Keys.ToList().ForEach(paramName =>
                            {
                                threeLineDiagCircuitIden.CircuitIdentifierFI.LookupParameter(paramName).Set(boolDict[paramName] ? 1 : 0);
                            });
                            continue;
                        }
                        if (boolDict.Keys.Count() != 0)
                        {
                            string paramName = boolDict.Keys.ToList()[0];
                            threeLineDiagCircuitIden.CircuitIdentifierFI.LookupParameter(paramName).Set(boolDict[paramName] ? 1 : 0);
                        }
                    }
                };
            }

            public void GenThreeLineDiagramFromPanel(ECs.Panel panel, ThreeLineDiagramBody threeLineDiagObj)
            {

                this.GetCircuitsInfosFromSpreadsheet(panel);
                FamilySymbol threelineDiagFS = this.diagrams.GetThreeLineDiagramBodySymbol("Trifasico");

                threeLineDiagObj.NomeDoQD.Add("Nome do QD", panel.Name);
                threeLineDiagObj.QtdeDeCircuitos.Add("Qtde circuitos", panel.AssignedCircuits.Count());
                threeLineDiagObj.PotenciaInstalada.Add("Potência instalada", this.totalLoad);
                threeLineDiagObj.PotenciaInstalada.Add("Potência demandada", this.demandedLoad);

                Transaction trans = new Transaction(doc);
                trans.Start("Generating Three-line diagram body");

                threeLineDiagObj.ThreeLineDiagramFI = this.doc.Create.NewFamilyInstance(new XYZ(), threelineDiagFS, this.threeLineView);
                SetupThreeLineDiagramBody(threeLineDiagObj);

                trans.Commit();


                double circYpos = 0;
                double rightCircXpos = ut.metersToFeet(4);
                double leftCircXpos = 0;
                bool sideFlag = false;
                FamilySymbol fsymCircuit;
                string tipoAlimentacao;

                foreach (ECs.Circuit circuit in panel.AssignedCircuits)
                {

                    switch (circuit.numOfPoles)
                    {
                        case 1:
                            tipoAlimentacao = "monofasico";
                            break;
                        case 2:
                            tipoAlimentacao = "bifasico";
                            break;
                        default:
                            tipoAlimentacao = "trifasico";
                            break;
                    }

                    if (sideFlag)
                    {
                        fsymCircuit = this.diagrams.GetThreeLineDiagramRightCircuitSymbol(tipoAlimentacao);
                    } else
                    {
                        fsymCircuit = this.diagrams.GetThreeLineDiagramLeftCircuitSymbol(tipoAlimentacao);
                    }

                    ThreeLineDiagramCircuitIdentifier circuitIdenData = new ThreeLineDiagramCircuitIdentifier();

                    circuitIdenData.Conexoes = circuitIdenData.GetConexoes(circuit);
                    circuitIdenData.CorrenteDisjuntor.Add("Corrente do Disjuntor", Convert.ToInt32(this.breakers[circuit.circuitNumber]));
                    circuitIdenData.DescricaoCircuito.Add("Descrição Circuito", circuit.Name);
                    circuitIdenData.NumeroDoCircuito.Add("Número Circuito", circuit.circuitNumber);
                    circuitIdenData.SeccaoCabos.Add("Secção dos cabos", this.cableSeccions[circuit.circuitNumber]);
                    circuitIdenData.Tensao.Add("Tensão", circuit.voltage);
                    circuitIdenData.Frequencia.Add("Frequência", 60);
                    circuitIdenData.Potencia.Add("Potência Circuito", circuit.apparentload);
                    circuitIdenData.EReserva.Add("Não Reserva", circuit.isNotReserveCircuit == 1);
                    circuitIdenData.TemDR.Add("DR Visível", circuit.TemDR == 1);
                    
                    
                    if (circuit.TemDR == 1)
                    {
                        circuitIdenData.CorrenteDoDR.Add("Corrente do DR", circuit.CorrenteSuportadaDR);
                        circuitIdenData.NumeroDePolosDR.Add("Número de polos DR", circuit.NumeroDePolosDR);
                        circuitIdenData.CorrenteDeProtecaoDR.Add("Corrente de proteção DR", circuit.CorrenteDeProtecaoDR);
                    }

                    XYZ pt = new XYZ(
                        sideFlag ? rightCircXpos : leftCircXpos,
                        -ut.metersToFeet(circYpos),
                        0
                    );

                    sideFlag = !sideFlag;
                    circYpos += 1.4;


                    Transaction circuitIdenTrans = new Transaction(this.doc);
                    circuitIdenTrans.Start($"Generating circuit {circuit.circuitNumber} identifier");

                    circuitIdenData.CircuitIdentifierFI = this.doc.Create.NewFamilyInstance(pt, fsymCircuit, this.threeLineView);
                    SetupThreeLineDiagramCircuitIdentifier(circuitIdenData);

                    circuitIdenTrans.Commit();

                }
                


            }
        }


        public static void CreateCircuit (
            Document doc,
            string name,
            WireType wireType,
            int temDR,
            int CorrenteDr,
            IList<ElementId> dispositives)
        {
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Creating circuit " + name);
                ElectricalSystem circuit = ElectricalSystem.Create(doc, dispositives, ElectricalSystemType.PowerCircuit);
                circuit.LookupParameter("Tipo de fiação").Set(wireType.Id);
                circuit.LookupParameter("Nome da carga").Set(name);
                circuit.LookupParameter("Tem DR").Set(temDR);
                circuit.LookupParameter("Corrente Suportada DR").Set(CorrenteDr);
                circuit.LookupParameter("Corrente de proteção DR").Set(30);

                t.Commit();
            }
        }
            
        public class GeneralShortAutomations : ProjectAutomations
        {
            public GeneralShortAutomations(Document doc) : base(doc) { }

            public void SetupCircuitsConnections(ElectricalEquipment panel)
            {
                foreach (ElectricalSystem circ in panel.GetAssignedElectricalSystems())
                {
                    int numOfGrounds = circ.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_GROUNDS_PARAM).AsInteger();
                    int numOfNeutrals = circ.get_Parameter(BuiltInParameter.RBS_ELEC_CIRCUIT_WIRE_NUM_NEUTRALS_PARAM).AsInteger();

                    Transaction t = new Transaction(this.doc);

                    t.Start("Setting circuit " + circ.Name);
                    if (circ.LoadName.ToLower().Contains("iluminação") || numOfGrounds == 0)
                    {
                        circ.LookupParameter("Circuito com Terra").Set(0);
                    }
                    else
                    {
                        circ.LookupParameter("Circuito com Terra").Set(1);
                    }

                    if (numOfNeutrals == 0)
                    {
                        circ.LookupParameter("Circuito com Neutro").Set(0);
                    }
                    else
                    {
                        circ.LookupParameter("Circuito com Neutro").Set(1);
                    }
                    t.Commit();
                }
            }

            public void SendRoomsDataToSheets(Document doc)
            {
                Utils ut = new Utils(doc);

                List<RevitLinkInstance> rli = ut.GetRevitLinks();

                foreach (RevitLinkInstance link in rli)
                {
                    Document linkDoc = link.GetLinkDocument();
                    List<Room> rooms = ut.getRoomsFromProject(linkDoc);

                }
            }
 
            public void StartIdentifySelectedConduits(UIApplication uiapp)
            {
                Document doc = uiapp.ActiveUIDocument.Document;
                Selection sel = uiapp.ActiveUIDocument.Selection;
                Utils ut = new Utils(doc);
                List<Element> conduitsRef = null;
                try
                {
                    conduitsRef = sel.PickElementsByRectangle().ToList();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
                {
                    return;
                }

                if (conduitsRef == null || conduitsRef.Count() == 0)
                {
                    return;
                }

                List<Conduit> conduits = conduitsRef
                    .Where(e => e.Category.Name == "Conduites")
                    .Select(e => e as Conduit)
                    .ToList();
                List<Conduit> taggableConduits = ut.FilterTaggableConduits(conduits);

                FamilySymbol fsym = ut.symbolIdForConduits();
                Transaction taggingT = new Transaction(doc, "Tagging conduits");
                
                List<ElementId> taggedRuns = new List<ElementId>();

                taggingT.Start();
                foreach (Conduit c in taggableConduits) {
                    if (taggedRuns.Contains(c.RunId))
                    {
                        continue;
                    }
                    Reference cRef = new Reference(c);
                    LocationCurve LocCur = c.Location as LocationCurve;
                    XYZ midPt = (LocCur.Curve.GetEndPoint(0) + LocCur.Curve.GetEndPoint(1)) / 2;
                    XYZ tagPt = midPt + (LocCur.Curve as Line).Direction.CrossProduct(XYZ.BasisZ) * ut.metersToFeet(0.15);

                    IndependentTag tag = IndependentTag.Create(this.doc, fsym.Id, this.doc.ActiveView.Id, cRef, false, TagOrientation.Horizontal, tagPt);
                    taggedRuns.Add(c.RunId);
                };
                taggingT.Commit();

            }

            public void changeDispositiveTagFor100Load (UIApplication uiapp)
            {
                Document doc = uiapp.ActiveUIDocument.Document;
                Utils ut = new Utils(doc);
                List<IndependentTag> tags = ut.GetIndependentTagsByName("Tag de N Circ Legenda Pt Tomada");
                
                tags.ForEach((IndependentTag e) =>
                {
                    List<FamilyInstance> taggedDispositives = e.GetTaggedLocalElements().Cast<FamilyInstance>().ToList();
                    if (taggedDispositives.Count != 0 && taggedDispositives.First().Category.Name.ToLower() != "dispositivos de iluminação")
                    {
                        try
                        {
                            if (taggedDispositives.First().LookupParameter("Potência Aparente (VA)").AsValueString() == "100 VA")
                            {
                                string fmsymName = ut.SymbolIdForPowerDispositivesBelow100load().Name;
                                ut.ChangeTagByName(e, fmsymName, fmsymName);
                            }
                        } catch (Exception ex)
                        {
                            TaskDialog.Show("Errors", $"Erro ao trocar tags: {ex.ToString()}");
                        }
                    }
                });
            }
        }


    }
    


}
