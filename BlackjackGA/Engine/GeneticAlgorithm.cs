using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlackjackGA.Engine;

namespace BlackjackGA.Engine
{
    class GeneticAlgorithm
    {
        public Func<Strategy, float> FitnessFunction { get; set; }
        public Func<GeneticAlgorithmProgress, Strategy, bool> ProgressCallback { get; set; }
        public Strategy BestSolution { get; set; }
        public int NumGenerationsNeeded { get; set; }

        private GeneticAlgorithmParameters currentEngineParams = new GeneticAlgorithmParameters();  // valores default

        private List<Strategy> currentGeneration = new List<Strategy>();

        private StrategyPool pool;
        private float totalFitness = 0;

        public GeneticAlgorithm(GeneticAlgorithmParameters userParams)
        {
            currentEngineParams = userParams;
        }

        public Strategy FindBestSolution()
        {
            Strategy returnStrategy = new Strategy();
            // Este código asume que el mejor fitness es el que tiene el mejor house edge
            float bestFitnessScoreAllTime = float.MinValue;
            float bestAverageFitnessScore = float.MinValue;
            int bestSolutionGenerationNumber = 0;
            int bestAverageFitnessGenerationNumber = 0;

            // Se crea un pool de candidatos (estrategias), para no crear y borrar en muchas ocasiones
            // Aquí se multiplica por 2 para cubrir esta generación y la próxima
            pool = new StrategyPool(currentEngineParams.PopulationSize * 2);     

            // Inicializar la primera generación (generación 0) con estrategias aleatorias o "Dummies"
            for (int n = 0; n < currentEngineParams.PopulationSize; n++)
            {
                var strategy = pool.GetRandomized();
                currentGeneration.Add(strategy);
            }

            int currentGenerationNumber = 0;

            while (true)
            {
                // Para cada candidato(estrategia) buscar el house edge(fitness score)
                Parallel.ForEach(currentGeneration, (candidate) =>
                {
                    // Calcular el house edge(fitness score) con la función de comparación de estrategias
                    candidate.Fitness = FitnessFunction(candidate);
                });

                // verificar si se encontró una mejor estrategia 
                float bestFitnessScoreThisGeneration = float.MinValue;
                Strategy bestSolutionThisGeneration = null;
                float totalFitness = 0;

                foreach (var candidate in currentGeneration)
                {
                    totalFitness += candidate.Fitness;

                    // Buscar el mejor candidato de esta generación y actualizar la mejor estrategia encontrada(si es el caso)
                    bool isBestThisGeneration = candidate.Fitness > bestFitnessScoreThisGeneration;
                    if (isBestThisGeneration)
                    {
                        bestFitnessScoreThisGeneration = candidate.Fitness;
                        bestSolutionThisGeneration = candidate;

                        bool isBestEver = bestFitnessScoreThisGeneration > bestFitnessScoreAllTime;
                        if (isBestEver)
                        {
                            bestFitnessScoreAllTime = bestFitnessScoreThisGeneration;
                            BestSolution = candidate.Clone();
                            bestSolutionGenerationNumber = currentGenerationNumber;
                        }
                    }
                }

                // Calcular el promedio y guardalo en caso de que sea el mejor
                float averageFitness = totalFitness / currentEngineParams.PopulationSize;
                if (averageFitness > bestAverageFitnessScore)
                {
                    bestAverageFitnessGenerationNumber = currentGenerationNumber;
                    bestAverageFitnessScore = averageFitness;
                }

                // Presentar el progreso al usuario
                GeneticAlgorithmProgress progress = new GeneticAlgorithmProgress()
                {
                    GenerationNumber = currentGenerationNumber,
                    AvgFitnessThisGen = averageFitness,
                    BestFitnessThisGen = bestFitnessScoreThisGeneration,
                    BestFitnessSoFar = bestFitnessScoreAllTime,
                };

                // Darle la opción al usuario de terminar el algoritmo con dado progreso
                bool keepGoing = ProgressCallback(progress, bestSolutionThisGeneration);
                if (!keepGoing) break;  // break en caso de que el usuario decida terminar

                // termination conditions
                /*
                 * TODO
                 */


                // Preparación para la próxima generación
                foreach (var strategy in currentGeneration)
                    pool.Release(strategy);
                

                currentGenerationNumber++;

            }


            return returnStrategy;

        }
    }
}
