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

            // Dependiendo del método de selección, puede ser que necesitemos ordenar los candidatos segun su fitness
            bool needToSortByFitness =
                currentGeneticAlgorithmParams.SelectionStyle == SelectionStyle.Roulette ||
                currentGeneticAlgorithmParams.SelectionStyle == SelectionStyle.Ranked;

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

                // Verificar si se encontró una mejor estrategia 
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

                // Termination conditions
                if (currentGenerationNumber >= currentGeneticAlgorithmParams.MinGenerations)
                {
                    // Salir del ciclo en caso se que no se vea progreso
                    if (((currentGenerationNumber - bestAverageFitnessGenerationNumber) >= currentGeneticAlgorithmParams.MaxStagnantGenerations) &&
                        ((currentGenerationNumber - bestSolutionGenerationNumber) >= currentGeneticAlgorithmParams.MaxStagnantGenerations))
                        break;

                    // Salir del ciclo en caso de que se llegue a la cantidad máxima de generaciones
                    if (currentGenerationNumber >= currentGeneticAlgorithmParams.MaxGenerations-1)
                        break;
                }

                // Dependiendo del método de selección, ordenamos los candidatos segun su fitness.
                AdjustFitnessScores(needToSortByFitness);
                
                // Preparación para la próxima generación
                nextGeneration.Clear();


                // Se hace selection y crossover para obtener los candidatos de la próxima generación
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

        private void AdjustFitnessScores(bool needToSortByFitness)
        {
            //Dependiendo del método de selección, ordenamos los candidatos segun su fitness.
            if (needToSortByFitness)
                currentGeneration = currentGeneration.OrderByDescending(c => c.Fitness).ToList();

            // En caso de que el método sea ranked, ajustamos los fitness para que sean ranks, por ejemplo, 0 sería el peor y N-1 el mejor.
            if (currentGeneticAlgorithmParams.SelectionStyle == SelectionStyle.Ranked)
            {
                float fitness = currentGeneration.Count - 1;
                foreach (var candidate in currentGeneration)
                    candidate.Fitness = fitness--;
            }

            // Calcular selección roulette y ranked, los casos en los cuales se necesita ordenar.
            totalFitness = 0;
            if (currentGeneticAlgorithmParams.SelectionStyle == SelectionStyle.Roulette ||
                currentGeneticAlgorithmParams.SelectionStyle == SelectionStyle.Ranked)
            {
                float smallestFitness = currentGeneration.Min(c => c.Fitness);
                float addToEach = 0;
                if (smallestFitness < 0)
                {
                    // En caso de que el menor fitness sea negativo, le añadimos el fitness mínimo a cada fitness para que solo tengamos valores positivos
                    addToEach = Math.Abs(smallestFitness);
                }

                foreach (var candidate in currentGeneration)
                {
                    candidate.Fitness += addToEach;
                    totalFitness += candidate.Fitness;
                }
            }
        }

        private Strategy[] SelectAndCrossover(int numNeeded)
        {
            // Procesamiento paralelo para obtener los candidatos de la próxima generación
            ConcurrentBag<Strategy> results = new ConcurrentBag<Strategy>();
            Parallel.For(0, numNeeded, (i) =>
            {
                var randomizer = new Randomizer(); 

                // Seleccionar los parents
                Strategy parent1 = null, parent2 = null;
                switch (currentGeneticAlgorithmParams.SelectionStyle)
                {
                    case SelectionStyle.Tourney:
                        parent1 = TournamentSelectParent();
                        parent2 = TournamentSelectParent();
                        break;

                    case SelectionStyle.Roulette:
                    case SelectionStyle.Ranked:
                        parent1 = RouletteAndRankedSelectParent();
                        parent2 = RouletteAndRankedSelectParent();
                        break;
                }
                // Hacer el cross over entre 2 parents para obtener el hijo
                Strategy child = pool.GetEmpty();
                parent1.CrossOverWith(parent2, child);

                // Hacer la mutación
                if (randomizer.RandomFloatFromZeroToOne() < currentGeneticAlgorithmParams.MutationRate)
                    child.Mutate(currentGeneticAlgorithmParams.MutationImpact);
                results.Add(child);
            });

            return results.ToArray();
        }

        private Strategy TournamentSelectParent()
        {
            var randomizer = new Randomizer(); 

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

        private Strategy RouletteAndRankedSelectParent()
        {
            var randomizer = new Randomizer();  

            // Para el método Roulette, utilizamos una probabilidad proporcional al fitness en comparación
            // al fitness total de todas las probabilidades
            double randomValue = randomizer.RandomDoubleFromZeroToOne() * totalFitness;
            for (int i = 0; i < currentGeneticAlgorithmParams.PopulationSize; i++)
            {
                randomValue -= currentGeneration[i].Fitness;
                if (randomValue <= 0)
                {
                    return currentGeneration[i];
                }
            }

            return currentGeneration[currentGeneticAlgorithmParams.PopulationSize - 1];
        }

    }
}
