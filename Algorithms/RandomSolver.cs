using System;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithms.Algorithms
{
    public class RandomSolver<TProblem, TSolution> : ISolver<TProblem, TSolution> where TSolution : ISolution
    {
        private readonly Func<TProblem, TSolution> generateRandomSolution;
        private readonly bool returnAllSolutions;

        public RandomSolver(Func<TProblem, TSolution> generateRandomSolution, bool returnAllSolutions = false)
        {
            this.generateRandomSolution = generateRandomSolution;
            this.returnAllSolutions = returnAllSolutions;
        }

        public IEnumerable<TSolution> GetSolutions(TProblem problem, Countdown countdown)
        {
            var steps = new List<TSolution> {generateRandomSolution(problem)};
            while (!countdown.IsFinished())
            {
                var solution = generateRandomSolution(problem);
                if (returnAllSolutions || solution.Score > steps.Last().Score)
                {
                    if (solution is IHaveTime withTime) withTime.Time = countdown.TimeElapsed;
                    steps.Add(solution);
                }
            }

            return steps;
        }
    }
}