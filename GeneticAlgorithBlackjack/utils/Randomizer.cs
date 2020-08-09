using System;
using System.Threading;

namespace BlackjackStrategy.utils
{
    public class Randomizer
    {
        private Random randomizer;

        public Randomizer()
        {
            // un forma optima de como conseguir una seed "random"
            randomizer = new Random(Guid.NewGuid().GetHashCode());
        }

        public int IntInRange(int lower, int upper)
        {
            // retorna un valor random en un rango (rango inclusivo) 
            return randomizer.Next(lower,upper);
        }

        public int Lesser(int upper)
        {
            // retorna un valor random menor que (no inclusivo)
            return randomizer.Next(upper);
        }

        public double randomDoubleFromZeroToOne()
        {
            //retorna un doblue entre 0 y 1.
            return randomizer.NextDouble();
        }

        public float randomFloatFromZeroToOne()
        {
            //retorna un float entre 0 y 1.
            return randomizer.NextDouble();
        }

    }
}