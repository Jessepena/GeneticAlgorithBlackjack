using System.ComponentModel;

namespace BlackjackGA.Engine
{

    public sealed class GeneticAlgorithmParameters
    {
        //Candidatos a seleccionar en el torneo
        public int TourneySize { get; set; } = 3;

        //Cantidad de candidatos por generación
        public int PopulationSize { get; set; } = 10;

        //Número mínimo de generaciones
        public int MinGenerations { get; set; } = 1;

        //Número máximo de generaciones
        public int MaxGenerations { get; set; } = 5;

        //Número máximo de generaciones sin progreso, tanto en el average, como en el mejor
        public int MaxStagnantGenerations { get; set; } = 2;


        public override string ToString()
        {
            return "";
        }
    }
}
