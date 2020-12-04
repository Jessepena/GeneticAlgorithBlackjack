using System.ComponentModel;

namespace BlackjackGA.Engine
{
    public enum SelectionStyle { Tourney, Roulette, Ranked };

    public sealed class GeneticAlgorithmParameters
    {
        //Tipo de selección a utilizar para el crossover
        public SelectionStyle SelectionStyle { get; set; } = SelectionStyle.Ranked;

        //Candidatos a seleccionar en el torneo
        public int TourneySize { get; set; } = 6;

        //Cantidad de candidatos por generación
        public int PopulationSize { get; set; } = 400;

        //Número mínimo de generaciones
        public int MinGenerations { get; set; } = 15;

        //Número máximo de generaciones
        public int MaxGenerations { get; set; } = 250;

        //Número máximo de generaciones sin progreso, tanto en el average, como en el mejor
        public int MaxStagnantGenerations { get; set; } = 10;
        
        //Porcentaje de candidatos que son afectados por la mutación (De 0.0 a 1.0)
        public double MutationRate { get; set; } = 0.10;

        //Porcentaje de mutación que sufren los candidatos (De 0.0 a 1.0)
        public double MutationImpact { get; set; } = 0.10;

        public override string ToString()
        {
            return "";
        }
    }
}
