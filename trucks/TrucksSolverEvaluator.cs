using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.Trucks
{
    public static class TrucksSolverEvaluator
    {
        public static double GetTotalScore(ISolver<TrucksProblem, TrucksSolution> solver, int timeoutMs, bool logScore = false)
        {
            var totalScore = 0.0;
            foreach (var problem in TrucksProblemRepo.GetTests())
            {
                var sw = Stopwatch.StartNew();
                var solutions = solver.GetSolutions(problem, timeoutMs).ToList();
                if (sw.ElapsedMilliseconds > timeoutMs + 50)
                    throw new Exception($"Solver spent {sw.ElapsedMilliseconds} ms on test {problem}. Time limit is {timeoutMs} ms.");
                if (logScore)
                {
                    Console.WriteLine(problem);
                    Console.WriteLine($"Last of {solutions.Count} solution:");
                    Console.WriteLine(solutions.Skip(solutions.Count - 3).StrJoin("\n"));
                    Console.WriteLine();
                }

                var solution = solutions.Last();
                totalScore += ValidateSolution(problem, solution);
            }
            return totalScore;
        }

        private static double ValidateSolution(TrucksProblem problem, TrucksSolution solution)
        {
            if (solution.Trucks.Length != problem.TrucksCount)
                throw new Exception("problem.Trucks.Count is wrong");
            var allBoxes = new HashSet<Box>();
            var minWeight = double.MaxValue;
            var maxWeight = double.MinValue;
            foreach (var truck in solution.Trucks)
            {
                foreach (var truckBox in truck.Boxes)
                    allBoxes.Add(truckBox);
                var volume = truck.Boxes.Sum(b => b.Volume);
                if (volume > problem.TruckVolume)
                    throw new Exception("Truck volume overflow");
                var weight= truck.Boxes.Sum(b => b.Weight);
                minWeight = Math.Min(minWeight, weight);
                maxWeight = Math.Max(maxWeight, weight);
            }

            if (allBoxes.Count != problem.Boxes.Length)
                throw new Exception("Wrong boxes!");
            allBoxes.IntersectWith(problem.Boxes);
            if (allBoxes.Count != problem.Boxes.Length)
                throw new Exception("Wrong boxes!");
            return minWeight - maxWeight;
        }
    }
}