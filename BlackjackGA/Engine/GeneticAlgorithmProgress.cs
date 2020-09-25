using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackGA.Engine
{
    class GeneticAlgorithmProgress
    {
        public int GenerationNumber { get; set; }

        public float BestFitnessThisGen { get; set; }

        public float AvgFitnessThisGen { get; set; }

        public float BestFitnessSoFar { get; set; }
    }
}
