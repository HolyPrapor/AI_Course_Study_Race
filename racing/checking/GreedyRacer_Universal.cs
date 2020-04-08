using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing.checking
{
    public enum Aggregation
    {
        Sum,
        Max,
        Last
    }

    public class GreedyRacer_Universal : ISolver<RaceState, RaceSolution>
    {
        private readonly IEstimator estimator;
        private readonly int simulationDepth;
        private readonly bool extendStrategyWithNop;
        private readonly bool seedWithHeuristics;

        public GreedyRacer_Universal(IEstimator estimator, int simulationDepth, bool extendStrategyWithNop, bool seedWithHeuristics = false)
        {
            this.estimator = estimator;
            this.simulationDepth = simulationDepth;
            this.extendStrategyWithNop = extendStrategyWithNop;
            this.seedWithHeuristics = seedWithHeuristics;
        }

        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var variants = new List<RaceSolution>();
            if (seedWithHeuristics)
            {
                var solution = new HeuristicsRacer().GetDeepSolution(problem, simulationDepth);
                solution.Score = estimator.Estimate(problem, solution);
                variants.Add(solution);
                //return variants;
            }

            foreach (var acceleration in GetAccelerations())
            {
                var accelerations = new[] { acceleration }
                    .Concat(Enumerable.Repeat(extendStrategyWithNop ? V.Zero : acceleration, simulationDepth)).ToArray();
                var solution = new RaceSolution(accelerations);
                solution.Score = estimator.Estimate(problem, solution);
                variants.Add(solution);
            }

            return variants.OrderBy(s => s.Score);
        }

        private IEnumerable<V> GetAccelerations()
        {
            return
                from x in Enumerable.Range(-1, 3)
                from y in Enumerable.Range(-1, 3)
                select new V(x, y);
        }
    }
}