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

        public MainWindow()
        {

            InitializeComponent();
           
            

        }

        private void btnBasicStrategy_Click(object sender, RoutedEventArgs e)
        {
            double average;
            double deviation;
            double coef;

            BasicStrategy basicStrategy = new BasicStrategy();
            basicStrategy.LoadStandardStrategy();
            TestConditions testConditions = new TestConditions();

            var game = new Game(basicStrategy, testConditions);
            double money = game.GetStrategyScore(testConditions.NumHandsToPlay);

            game.GetStatistics(out average, out deviation, out coef);
            writeAllforStrategy(testConditions,"Estrategia básica", average, deviation, coef);
            DisplayStrategyGrids(basicStrategy, "Estrategia Básica");
        }

        private void btnIllustrious_Click(object sender, RoutedEventArgs e)
        {
            double average;
            double deviation;
            double coef;

            Illustrious18 strategy = new Illustrious18();
            strategy.LoadStandardStrategy();
            TestConditions testConditions = new TestConditions();

            var game = new Game(strategy, testConditions);
            double money = game.GetStrategyScore(testConditions.NumHandsToPlay);

            game.GetStatistics(out average, out deviation, out coef);
            writeAllforStrategy(testConditions, "Estrategia Illustrious", average, deviation, coef);
            DisplayStrategyGrids(strategy, "Estrategia Illustrious");
        }

        private void writeAllforStrategy(TestConditions testConditions, string strategyName, double average, double deviation, double coef )
        {
            Console.WriteLine("RESULTADOS " + strategyName);
            Console.WriteLine("El average es: " + average);

            double x = (average / testConditions.NumHandsToPlay * testConditions.BetSize);

            Console.WriteLine("El house edge es: " + x);
            Console.WriteLine("La desviacion es: " + deviation);
            Console.WriteLine("El coef de variacion es: " + coef);

        }

        private void btnGeneticAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            
            // Re-Creamos el archivo con resultados al tratar de escribir un string vacio dentro de el.
            string resultados = "";
            //System.IO.File.WriteAllText(@"D:\Desktop\Proyecto\GeneticAlgorithBlackjack\resultados.txt", resultados);
            

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
            //AppendTextToFile(progress.AvgFitnessThisGen);
            Console.WriteLine("Mejor fitness de esta generación: " + progress.BestFitnessThisGen);
            Console.WriteLine("Mejor fitness de todas las generaciones hasta ahora: " + progress.BestFitnessSoFar);
            DisplayStrategyGrids(bestThisGen, "Generación #" + progress.GenerationNumber);
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
            new System.IO.StreamWriter(@"D:\Desktop\Proyecto\GeneticAlgorithBlackjack\resultados.txt", true))
            {
                file.WriteLine(fitness.ToString());
            }
        }

        

        private void DisplayStrategyGrids(StrategyBase strategy, string caption)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                string imgFilename = "lastGen";
                StrategyPrint.ShowPlayableHands(strategy, canvas, imgFilename, caption);
            }),
            DispatcherPriority.Background);

        }

      
    }

}
