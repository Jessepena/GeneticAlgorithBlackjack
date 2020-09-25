using System.ComponentModel;

namespace BlackjackGA.Engine
{

    public sealed class GeneticAlgorithmParameters
    {
        //Cantidad de candidatos por generación
        public int PopulationSize { get; set; } = 50;

        public override string ToString()
        {
            return "";
        }
    }
}
