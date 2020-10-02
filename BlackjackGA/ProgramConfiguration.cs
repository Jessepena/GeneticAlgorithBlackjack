using BlackjackGA.Engine;
using BlackjackGA.Utils;

namespace BlackjackGA
{
    public class ProgramSettings
    {
        public TestConditions TestSettings { get; set; } = new TestConditions();
        public GeneticAlgorithmParameters GAsettings { get; set; } = new GeneticAlgorithmParameters();
    }
}
