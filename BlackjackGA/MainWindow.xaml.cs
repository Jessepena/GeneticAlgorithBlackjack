using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using BlackjackGA.Representation;
using BlackjackGA.Engine;
using BlackjackGA.Utils;
using System.Windows.Controls;

namespace BlackjackGA
{
    
    public partial class MainWindow : Window
    {
        public ProgramSettings ProgramConfiguration { get; set; } = new ProgramSettings();
        private Stopwatch stopwatch = new Stopwatch();
        private Canvas canvas;

        public MainWindow()
        {

            InitializeComponent();
           

            BasicStrategy basicStrategy = new BasicStrategy();
            Illustrious18 lavaina = new Illustrious18();
            TestConditions testConditions = new TestConditions();

            double average;
            double deviation;
            double coef;
            
            var game = new Game(lavaina, testConditions);
            double money = game.GetStrategyScore(testConditions.NumHandsToPlay);
            


            Console.WriteLine("RESULTADOS BASIC STRATEGY");
            game.GetStatistics(out average, out deviation, out coef);
            Console.WriteLine("El average es: " + average);
            double x = (average / testConditions.NumHandsToPlay * testConditions.BetSize);
            Console.WriteLine("El house edge es: " + x);
            Console.WriteLine("La desviacion es: " + deviation);
            Console.WriteLine("El coef de variacion es: " + coef);

            DisplayStrategyGrids(basicStrategy, "test");



        }

        private void btnSearchBestAlgorithm()
        {
            // Re-Creamos el archivo con resultados al tratar de escribir un string vacio dentro de el.
            string resultados = "";
            System.IO.File.WriteAllText(@"D:\Desktop\GeneticAlgorithBlackjack\resultados.txt", resultados);

            // Empieza el conteo. 
            stopwatch.Restart();

            // Busca la solucion de la mejor estrategia dado el Genetic Algorithm
            Task.Factory.StartNew(() => AsyncFindSolutionAndShowResults());
            stopwatch.Stop();
        }

        private bool PerGenerationCallBack(GeneticAlgorithmProgress progress, Strategy bestThisGen)
        {
            
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Generación #" + progress.GenerationNumber);
            Console.WriteLine("Promedio de fitness de esta generación: " + progress.AvgFitnessThisGen);
            AppendTextToFile(progress.AvgFitnessThisGen);
            Console.WriteLine("Mejor fitness de esta generación: " + progress.BestFitnessThisGen);
            Console.WriteLine("Mejor fitness de todas las generaciones hasta ahora: " + progress.BestFitnessSoFar);

            return true;
        }



        private void AsyncFindSolutionAndShowResults()
        {
            stopwatch.Restart();
            var geneticAlgorithm = new GeneticAlgorithm(ProgramConfiguration.GAsettings);
            geneticAlgorithm.ProgressCallback = PerGenerationCallBack;
            geneticAlgorithm.FitnessFunction = EvaluateCandidate;
            
            double generatedStrategyScore = EvaluateCandidate(geneticAlgorithm.FindBestSolution());

            stopwatch.Stop();
            Console.WriteLine("Tiempo empleado:");
            Console.WriteLine("Minutos: " + stopwatch.Elapsed.Minutes);
            Console.WriteLine("Segundos: " + stopwatch.Elapsed.Seconds);
            Console.WriteLine("Milisegundos: " + stopwatch.Elapsed.Milliseconds);

        }

        private float EvaluateCandidate(Strategy candidate)
        {
            var game = new Game(candidate, ProgramConfiguration.TestSettings);
            return game.GetStrategyScore(ProgramConfiguration.TestSettings.NumHandsToPlay);
        }

        private void AppendTextToFile(float fitness)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"D:\Desktop\GeneticAlgorithBlackjack\resultados.txt", true))
            {
                file.WriteLine(fitness.ToString());
            }
        }

        

        private void DisplayStrategyGrids(StrategyBase strategy, string caption)
        {
            string imgFilename = "lastGen";
            StrategyPrint.ShowPlayableHands(strategy, canvas, imgFilename, caption);
           
        }
    }

}
