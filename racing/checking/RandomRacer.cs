using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing.checking
{
    public class RandomRacer : ISolver<RaceState, RaceSolution>
    {
        private readonly int depth;
        private readonly double changeCommandChance;
        private readonly IEstimator estimator;
        private readonly Random random;
        public StatValue SimCount { get; } = new StatValue();

        public RandomRacer(int depth, double changeCommandChance, IEstimator estimator, Random random)
        {
            this.depth = depth;
            this.changeCommandChance = changeCommandChance;
            this.estimator = estimator;
            this.random = random;
        }

        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var variants = new List<RaceSolution>();

            var heuristicsSolution = new HeuristicsRacer().GetDeepSolution(problem, depth);
            heuristicsSolution.Score = estimator.Estimate(problem, heuristicsSolution);
            variants.Add(heuristicsSolution);

            var simCount = 0;
            var commands = GetAccelerations().ToList();
            while (!countdown.IsFinished())
            {
                var aa = new List<V>();
                var lastA = random.Choice(commands);
                aa.Add(lastA);
                for (int i = 0; i < depth; i++)
                {
                    lastA = random.Chance(changeCommandChance)
                        ? random.Choice(commands)
                        : lastA;
                    aa.Add(lastA);
                }
                var solution = new RaceSolution(aa.ToArray());
                solution.Score = estimator.Estimate(problem, solution); 
                if (variants.Count == 0 || variants.Last().Score < solution.Score)
                    variants.Add(solution);
                simCount++;
            }

            SimCount.Add(simCount);
            return variants;
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