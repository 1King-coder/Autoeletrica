using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace PlayingWithMEP.Sources
{
    internal interface IElectricalUtilityData
    {
        int CorrenteDisjuntor { get; set; }
        double SeccaoCabos { get; set; }

    }

    internal interface ICircuitsIdentifierData
    {
        int CorrenteDisjuntor { get; set; }
        string Descricao { get; set; }
        int NaoReserva { get; set; }
        string NumeroCircuito { get; set; }
        int Potencia { get; set; }
        string SeccaoCabos { get; set; }

        Dictionary<string, string> circuitLoadPerPhase { get; set; }

        string GetPhasesWithLoad();

    }

    internal interface IThreeLineCircuitsIdentifierData
    {
        int CorrenteDisjuntor { get; set; }
        string Descricao { get; set; }
        int NaoReserva { get; set; }
        string NumeroCircuito { get; set; }

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
