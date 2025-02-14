﻿using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace AutoEletrica.Sources
{
    internal interface IElectricalUtilityData
    {
        int CorrenteDisjuntor { get; set; }
        double SeccaoCabos { get; set; }

    }

    internal interface IThreeLineDiagramBody
    {
        FamilyInstance ThreeLineDiagramFI { get; set; }
        Dictionary<string, int> CorrenteDR { get; set; }
        Dictionary<string, int> CorrenteDisjuntorGeral { get; set; }
        Dictionary<string, int> CorrenteDeProtecaoDR { get; set; }
        Dictionary<string, int> CorrenteDeCurtoCircuito { get; set; }
        Dictionary<string, int> Frequencia { get; set; }
        Dictionary<string, int> Tensao { get; set; }
        Dictionary<string, string> NomeDoQD { get; set; }
        Dictionary<string, bool> TemDPSParaNeutro { get; set; }
        Dictionary<string, int> QtdeDeCircuitos { get; set; }
        Dictionary<string, string> SeccaoCabos { get; set; }
        Dictionary<string, bool> TemDPS { get; set; }
        Dictionary<string, bool> TemDR { get; set; }
        Dictionary<string, int> CorrenteDeProtecaoDPS { get; set; }
        Dictionary<string, int> TensaoNominalDPS { get; set; }
        Dictionary<string, string> ClasseDeProtecaoDPS { get; set; }
    }


    internal interface ICircuitsIdentifierData
    {
        int CorrenteDisjuntor { get; set; }
        string Descricao { get; set; }
        int NaoReserva { get; set; }
        string NumeroCircuito { get; set; }
        int Potencia { get; set; }
        string SeccaoCabos { get; set; }

        int Tensao {  get; set; }

        int Frequencia {  get; set; }

        Dictionary<string, string> circuitLoadPerPhase { get; set; }

        string GetPhasesWithLoad();

    }

    internal interface IThreeLineCircuitsIdentifierData
    {
        int CorrenteDisjuntor { get; set; }
        string Descricao { get; set; }
        int NaoReserva { get; set; }
        string NumeroCircuito { get; set; }

        int Tensao { get; set; }

        int Frequencia { get; set; }

        List<string> Potencias { get; set; }

        Dictionary<string, string> circuitLoadPerPhase { get; set; }

        List<string> GetLoadList();

        string GetPhasesWithLoad();

    }

    internal interface IThreeLinePanelIdentifierData
    {
        int CorrenteDisjuntor { get; set; }
        string SeccaoCabos { get; set; }
        int numOfPoles { get; set; }
        int numOfCircuits { get; set; }

        string name { get; set; }
    }

    internal interface IPanelIdentifierData
    {
        int CorrenteDisjuntorGeral { get; set; }
        string ClasseDeProtecaoDPS { get; set; }
        int CorrenteNominalDPS { get; set; }
        int TensaoNominalDPS { get; set; }
        int CorrenteDR { get; set; }
        int CorrenteProtecaoDR { get; set; }
        int NumeroPolosDR { get; set; }
        double SeccaoCabos { get; set; }
        int DPSneutro { get; set; }

        int HasDPS {  get; set; }

        int HasGeneralDR { get; set; }
    }

    internal class ElectricalUtilityData : IElectricalUtilityData
    {
        public int CorrenteDisjuntor { get; set; }

        public double SeccaoCabos { get; set; }
    }

    internal class CircuitsIdentifierData : ICircuitsIdentifierData
    {
        public int CorrenteDisjuntor { get; set; }
        public string Descricao { get; set; }
        public int NaoReserva { get; set; }
        public string NumeroCircuito { get; set; }
        public int Potencia { get; set; }
        public string SeccaoCabos { get; set; }
        public int Tensao { get; set; }

        public int Frequencia { get; set; }

        public Dictionary<string, string> circuitLoadPerPhase { get; set; }

        public string GetPhasesWithLoad()
        {
            string result = "";

            string phaseA = this.circuitLoadPerPhase["A"].Equals("0") ? "" : "A";
            string phaseB = this.circuitLoadPerPhase["B"].Equals("0") ? "" : "B";
            string phaseC = this.circuitLoadPerPhase["C"].Equals("0") ? "" : "C";

            string[] phases = { phaseA, phaseB, phaseC };

            for (int i = 0; i < 3; i++)
            {
                if (string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(phases[i]))
                {
                    result += phases[i];
                    continue;
                }
                if (!string.IsNullOrEmpty(phases[i]))
                {
                    result += $", {phases[i]}";
                }


            }

            return result;
        }

    }

    internal class ThreeLineDiagramBody : IThreeLineDiagramBody
    {
        public FamilyInstance ThreeLineDiagramFI { get; set; }
        public Dictionary<string, int> CorrenteDR { get; set; }
        public Dictionary<string, int> CorrenteDisjuntorGeral { get; set; }
        public Dictionary<string, string> SeccaoCabos { get; set; }
        public Dictionary<string, int> CorrenteDeProtecaoDR { get; set; }
        public Dictionary<string, int> CorrenteDeCurtoCircuito { get; set; }
        public Dictionary<string, int> Frequencia { get; set; }
        public Dictionary<string, int> Tensao { get; set; }
        public Dictionary<string, string> NomeDoQD { get; set; }
        public Dictionary<string, bool> TemDPSParaNeutro { get; set; }
        public Dictionary<string, int> QtdeDeCircuitos { get; set; }
        public Dictionary<string, bool> TemDPS { get; set; }
        public Dictionary<string, bool> TemDR { get; set; }
        public Dictionary<string, int> CorrenteDeProtecaoDPS { get; set; }
        public Dictionary<string, int> TensaoNominalDPS { get; set; }
        public Dictionary<string, string> ClasseDeProtecaoDPS { get; set; }

        public ThreeLineDiagramBody ()
        {
            CorrenteDR = new Dictionary<string, int>();
            CorrenteDisjuntorGeral = new Dictionary<string, int>();
            SeccaoCabos = new Dictionary<string, string>();
            CorrenteDeProtecaoDR = new Dictionary<string, int>();
            CorrenteDeCurtoCircuito = new Dictionary<string, int>();
            Frequencia = new Dictionary<string, int>();
            Tensao = new Dictionary<string, int>();
            NomeDoQD = new Dictionary<string, string>();
            TemDPSParaNeutro = new Dictionary<string, bool>();
            QtdeDeCircuitos = new Dictionary<string, int>();
            TemDPS = new Dictionary<string, bool>();
            TemDR = new Dictionary<string, bool>();
            CorrenteDeProtecaoDPS = new Dictionary<string, int>();
            TensaoNominalDPS = new Dictionary<string, int>();
            ClasseDeProtecaoDPS = new Dictionary<string, string>();
        }

    }

    internal class PanelIdentifierData : IPanelIdentifierData
    {
        public int CorrenteDisjuntorGeral { get; set; }
        public string ClasseDeProtecaoDPS { get; set; }
        public int CorrenteNominalDPS { get; set; }
        public int TensaoNominalDPS { get; set; }
        public int CorrenteDR { get; set; }
        public int CorrenteProtecaoDR { get; set; }
        public int NumeroPolosDR { get; set; }
        public double SeccaoCabos { get; set; }

        public int DPSneutro { get; set; }

        public int HasDPS { get; set; }

        public int HasGeneralDR {  get; set; }
    }
    internal class ThreeLineCircuitsIdentifierData : IThreeLineCircuitsIdentifierData
    {
        public int CorrenteDisjuntor { get; set; }
        public string Descricao { get; set; }
        public int NaoReserva { get; set; }
        public string NumeroCircuito { get; set; }
        public int numOfPoles { get; set; }

        public List<string> Potencias { get; set; }

        public int totalLoad { get; set; }

        public int Tensao {  get; set; }

        public int Frequencia { get; set; }

        public Dictionary<string, string> circuitLoadPerPhase { get; set; }

        public List<string> GetLoadList()
        {
            return new List<string>()
            {
                circuitLoadPerPhase["A"],
                circuitLoadPerPhase["B"],
                circuitLoadPerPhase["C"],
            };
        }
        public string GetPhasesWithLoad()
        {
            string result = "";

            string phaseA = this.circuitLoadPerPhase["A"].Equals("0") ? "" : "A";
            string phaseB = this.circuitLoadPerPhase["B"].Equals("0") ? "" : "B";
            string phaseC = this.circuitLoadPerPhase["C"].Equals("0") ? "" : "C";

            string[] phases = { phaseA, phaseB, phaseC };

            for (int i = 0; i < 3; i++)
            {
                if (string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(phases[i]))
                {
                    result += phases[i];
                    continue;
                }
                if (!string.IsNullOrEmpty(phases[i]))
                {
                    result += $", {phases[i]}";
                }


            }

            return result;
        }
    }
    internal class ThreeLinePanelIdenfierData : IThreeLinePanelIdentifierData
    {
        public int CorrenteDisjuntor { get; set; }
        public string SeccaoCabos { get; set; }
        public int numOfPoles { get; set; }
        public int numOfCircuits { get; set; }
        public string name { get; set; }
    }
}
