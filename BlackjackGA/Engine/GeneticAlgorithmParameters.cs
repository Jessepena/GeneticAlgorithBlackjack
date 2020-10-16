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
        public int PopulationSize { get; set; } = 100;

        //Número mínimo de generaciones
        public int MinGenerations { get; set; } = 15;

        //Número máximo de generaciones
        public int MaxGenerations { get; set; } = 50;

        //Número máximo de generaciones sin progreso, tanto en el average, como en el mejor
        public int MaxStagnantGenerations { get; set; } = 5;


        public override string ToString()
        {
            return "";
        }
    }
}
