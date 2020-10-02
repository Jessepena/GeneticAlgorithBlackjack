using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlackjackGA.Engine;
using BlackjackGA.Utils;

namespace BlackjackGA.Engine
{
    class GeneticAlgorithm
    {
        public Func<Strategy, float> FitnessFunction { get; set; }
        public Func<GeneticAlgorithmProgress, Strategy, bool> ProgressCallback { get; set; }
        public Strategy BestSolution { get; set; }
        public int NumGenerationsNeeded { get; set; }

        private GeneticAlgorithmParameters currentGeneticAlgorithmParams = new GeneticAlgorithmParameters();  // valores default

        private List<Strategy> currentGeneration = new List<Strategy>();

        private StrategyPool pool;
        private float totalFitness = 0;

        public GeneticAlgorithm(GeneticAlgorithmParameters userParams)
        {
            currentGeneticAlgorithmParams = userParams;
        }

        public Strategy FindBestSolution()
        {
            // Este código asume que el mejor fitness es el que tiene el mejor house edge
            float bestFitnessScoreAllTime = float.MinValue;
            float bestAverageFitnessScore = float.MinValue;
            int bestSolutionGenerationNumber = 0;
            int bestAverageFitnessGenerationNumber = 0;

            // una lista que representa los candidatos de la próxima generación
            List<Strategy> nextGeneration = new List<Strategy>();

            // Se crea un pool de candidatos (estrategias), para no crear y borrar en muchas ocasiones
            // Aquí se multiplica por 2 para cubrir esta generación y la próxima
            pool = new StrategyPool(currentGeneticAlgorithmParams.PopulationSize * 2);     

            // Inicializar la primera generación (generación 0) con estrategias aleatorias o "Dummies"
            for (int n = 0; n < currentGeneticAlgorithmParams.PopulationSize; n++)
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
                float averageFitness = totalFitness / currentGeneticAlgorithmParams.PopulationSize;
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
                if (currentGenerationNumber >= currentGeneticAlgorithmParams.MinGenerations)
                {
                    // Salir del ciclo en caso se que no se vea progreso
                    if (((currentGenerationNumber - bestAverageFitnessGenerationNumber) >= currentGeneticAlgorithmParams.MaxStagnantGenerations) &&
                        ((currentGenerationNumber - bestSolutionGenerationNumber) >= currentGeneticAlgorithmParams.MaxStagnantGenerations))
                        break;

                    // salir del ciclo en caso de que se llegue a la cantidad máxima de generaciones
                    if (currentGenerationNumber >= currentGeneticAlgorithmParams.MaxGenerations)
                        break;
                }

                // Preparación para la próxima generación
                nextGeneration.Clear();


                // then do the selection, crossover and mutation to populate the rest of the next generation
                var children = SelectAndCrossover(currentGeneticAlgorithmParams.PopulationSize);
                nextGeneration.AddRange(children);

                // Ir a la nueva generación
                foreach (var strategy in currentGeneration)
                    pool.Release(strategy);
                currentGeneration.Clear();
                currentGeneration.AddRange(nextGeneration);

                currentGenerationNumber++;

            }


            return BestSolution;

        }

        private Strategy[] SelectAndCrossover(int numNeeded)
        {
            // multi-threading to fill in the remaining children for the next generation
            ConcurrentBag<Strategy> results = new ConcurrentBag<Strategy>();
            Parallel.For(0, numNeeded, (i) =>
            {
                var randomizer = new Randomizer();  // thread-specific version

                // select parents
                Strategy parent1 = null, parent2 = null;

                parent1 = TournamentSelectParent();
                parent2 = TournamentSelectParent();

                // cross them over to generate a new child
                Strategy child = pool.GetEmpty();
                parent1.CrossOverWith(parent2, child);


                results.Add(child);
            });

            return results.ToArray();
        }

        private Strategy TournamentSelectParent()
        {
            var randomizer = new Randomizer();  // thread-specific version

            Strategy result = null;
            float bestFitness = float.MinValue;

            for (int i = 0; i < currentGeneticAlgorithmParams.TourneySize; i++)
            {
                int index = randomizer.Lesser(currentGeneticAlgorithmParams.PopulationSize);
                var randomCandidate = currentGeneration[index];
                var fitness = randomCandidate.Fitness;

                bool isFitnessBetter = fitness > bestFitness;
                if (isFitnessBetter)
                {
                    result = randomCandidate;
                    bestFitness = fitness;
                }
            }
            return result;
        }



    }
}
