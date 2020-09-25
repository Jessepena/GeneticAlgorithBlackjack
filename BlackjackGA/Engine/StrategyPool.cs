using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace BlackjackGA.Engine
{
    class StrategyPool
    {
        private object threadLock = new object();
        private Queue<Strategy> availablePool;
        private List<Strategy> inUseList;

        public StrategyPool(int populationSize)
        {
            // Una cola para obtener la próxima estrategia disponible, una lista para saber cuales se están usando
            availablePool = new Queue<Strategy>(populationSize);
            inUseList = new List<Strategy>(populationSize);

            // Popular el queue con nuevas estrategias
            for (int i = 0; i < populationSize; i++)
                availablePool.Enqueue(new Strategy());
        }

        public Strategy GetEmpty()
        {
            lock (threadLock)
            {
                if (!availablePool.Any())
                    Console.WriteLine("Cola vacía");

                var result = availablePool.Dequeue();
                inUseList.Add(result);

                return result;
            }
        }

        public Strategy GetRandomized()
        {
            lock (threadLock)
            {
                if (!availablePool.Any())
                    Console.WriteLine("Cola vacía"); 

                var result = availablePool.Dequeue();
                inUseList.Add(result);

                result.Randomize();
                return result;
            }
        }

        public Strategy CopyOf(Strategy strategy)
        {
            lock (threadLock)
            {
                if (!availablePool.Any())
                    Console.WriteLine("Cola vacía");

                var result = availablePool.Dequeue();
                inUseList.Add(result);

                result.DeepCopy(strategy);
                return result;
            }
        }

        public void Release(Strategy strategy)
        {
            var found = inUseList.Find(s => s == strategy);
            if(found == null)
                    Console.WriteLine("No se pudo liberar la estrategia");

            inUseList.Remove(found);
            availablePool.Enqueue(found);
        }
    }
}
