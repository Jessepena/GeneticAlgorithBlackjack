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


namespace BlackjackGA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public ProgramSettings ProgramConfiguration { get; set; } = new ProgramSettings();
        private Stopwatch stopwatch = new Stopwatch();
        
        public MainWindow()
        {

            InitializeComponent();
            
            BasicStrategy basicStrategy = new BasicStrategy();
            TestConditions testConditions = new TestConditions();

            double average;
            double deviation;
            double coef;
            
            var game = new Game(basicStrategy, testConditions);
            double money = game.GetStrategyScore(testConditions.NumHandsToPlay);
            


            Console.WriteLine("RESULTADOS BASIC STRATEGY");
            game.GetStatistics(out average, out deviation, out coef);
            Console.WriteLine("El average es: " + average);
            double x = (average / testConditions.NumHandsToPlay * testConditions.BetSize);
            Console.WriteLine("El house edge es: " + x);
            Console.WriteLine("La desviacion es: " + deviation);
            Console.WriteLine("El coef de variacion es: " + coef);
            stopwatch.Restart();
            string resultados = "";
            System.IO.File.WriteAllText(@"D:\Desktop\GeneticAlgorithBlackjack\resultados.txt", resultados);
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
    }

}
