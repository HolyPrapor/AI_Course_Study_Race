using System;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.Trucks
{
    public class TrucksSolution : IHaveTime, ISolution, IHaveIndex
    {
        public string Hint = "";
        public readonly Truck[] Trucks;

        public TrucksSolution(Truck[] trucks, TrucksProblem problem)
        :this(trucks, problem, GetScore(trucks, problem))
        {
            Trucks = trucks;
            Problem = problem;
        }

        private static double GetScore(Truck[] trucks, TrucksProblem problem)
        {
            if (trucks.Any(t => t.UsedVolume > problem.TruckVolume))
                return double.NegativeInfinity;
            return trucks.Min(t => t.UsedWeight) - trucks.Max(t => t.UsedWeight);
        }

        public TrucksSolution(Truck[] trucks, TrucksProblem problem, double score)
        {
            Problem = problem;
            Trucks = trucks;
            WorstTruckIndex = Enumerable.Range(0, problem.TrucksCount)
                .MaxBy(t => Trucks[t].UsedWeight.Distance(problem.TargetTruckWeight));
            Score = score;
        }

        public TrucksProblem Problem { get; }
        public int WorstTruckIndex { get; set; }
        public TimeSpan Time { get; set; }
        public double Score { get; }

        public override string ToString()
        {
            return $"{Score} {Hint} {Time} Improvement: {ImprovementIndex} Mutation: {MutationIndex}";
        }

        public int MutationIndex { get; set; }
        public int ImprovementIndex { get; set; }
    }
}