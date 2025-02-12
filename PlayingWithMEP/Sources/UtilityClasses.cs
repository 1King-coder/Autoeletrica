using Autodesk.Revit.DB;
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
        int CorrenteDR { get; set; }
        int CorrenteDisjuntorGeral { get; set; }
        int CorrenteDeProtecaoDR { get; set; }
        int CorrenteDeCurtoCircuito { get; set; }
        int Frequencia { get; set; }
        int Tensao { get; set; }
        string NomeDoQD { get; set; }
        bool TemDPSParaNeutro { get; set; }
        int QtdeDeCircuitos { get; set; }
        bool TemDPS { get; set; }
        bool TemDR { get; set; }

        // Declare methods to set the parameters of the interface directly in revit using lookupParameter
        void SetParamCorrenteDR(int value);

        void SetParamCorrenteDisjuntorGeral(int value);

        void SetParamCorrenteDeProtecaoDR(int value);

        void SetParamCorrenteDeCurtoCircuito(int value);

        void SetParamFrequencia(int value);

        void SetParamTensao(int value);

        void SetParamNomeDoQD(string value);

        void SetParamTemDPSParaNeutro(bool value);

        void SetParamQtdeDeCircuitos(int value);

        void SetParamTemDPS(bool value);

        void SetParamTemDR(bool value);
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
        public int CorrenteDR { get; set; }
        public int CorrenteDisjuntorGeral { get; set; }
        public int CorrenteDeProtecaoDR { get; set; }
        public int CorrenteDeCurtoCircuito { get; set; }
        public int Frequencia { get; set; }
        public int Tensao { get; set; }
        public string NomeDoQD { get; set; }
        public bool TemDPSParaNeutro { get; set; }
        public int QtdeDeCircuitos { get; set; }
        public bool TemDPS { get; set; }
        public bool TemDR { get; set; }

        public void SetParamCorrenteDR(int value)
        {
            CorrenteDR = value;
            ThreeLineDiagramFI.LookupParameter("Corrente DR").Set(value);
        }

        public void SetParamCorrenteDisjuntorGeral(int value)
        {
            CorrenteDisjuntorGeral = value;
            ThreeLineDiagramFI.LookupParameter("Corrente Disjuntor Geral").Set(value);
        }

        public void SetParamCorrenteDeProtecaoDR(int value)
        {
            CorrenteDeProtecaoDR = value;
            ThreeLineDiagramFI.LookupParameter("Corrente de Proteção DR").Set(value);
        }

        public void SetParamCorrenteDeCurtoCircuito(int value)
        {
            CorrenteDeCurtoCircuito = value;
            ThreeLineDiagramFI.LookupParameter("Corrente de curto-circuito").Set(value);
        }

        public void SetParamFrequencia(int value)
        {
            Frequencia = value;
            ThreeLineDiagramFI.LookupParameter("Frequência").Set(value);
        }

        public void SetParamTensao(int value)
        {
            Tensao = value;
            ThreeLineDiagramFI.LookupParameter("Tensão").Set(value);
        }

        public void SetParamNomeDoQD(string value)
        {
            NomeDoQD = value;
            ThreeLineDiagramFI.LookupParameter("Nome do QD").Set(value);
        }

        public void SetParamTemDPSParaNeutro(bool value)
        {
            TemDPSParaNeutro = value;
            ThreeLineDiagramFI.LookupParameter("DPS para o neutro").Set(value ? 1 : 0);
        }

        public void SetParamQtdeDeCircuitos(int value)
        {
            QtdeDeCircuitos = value;
            ThreeLineDiagramFI.LookupParameter("Qtde circuitos").Set(value);
        }

        public void SetParamTemDPS(bool value)
        {
            TemDPS = value;
            ThreeLineDiagramFI.LookupParameter("Tem DPS").Set(value ? 1 : 0);
        }

        public void SetParamTemDR(bool value)
        {
            TemDR = value;
            ThreeLineDiagramFI.LookupParameter("Tem DR").Set(value ? 1 : 0);
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
