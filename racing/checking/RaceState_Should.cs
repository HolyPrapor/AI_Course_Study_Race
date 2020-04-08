using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AiAlgorithms.Algorithms;
using NUnit.Framework;

namespace AiAlgorithms.racing.checking
{
    [TestFixture]
    public class RaceState_Should
    {
        [Test]
        public void RaceRandom()
        {
            var racer = new RandomRacer(15, 0.1, new RaceEstimator(Aggregation.Max), new Random(13123));
            RaceController.Play(RandomTrack(), racer, true);
            Console.WriteLine(racer.SimCount.ToDetailedString());
        }

        [Test]
        public void RaceGreedy()
        {
            var greedy = new GreedyRacer_Universal(new RaceEstimator(Aggregation.Max), 20, false, true);
            RaceController.Play(RandomTrack(), greedy, true);
        }

        [Test]
        public void RaceHeuristics()
        {
            RaceController.Play(RandomTrack(), new HeuristicsRacer(), true);
        }

        private static RaceState RandomTrack()
        { 
            //return RaceProblemsRepo.GenerateLine(100, 20, new Random(3243145));
            //return RaceProblemsRepo.Generate(50, 4, new Random(3243145));
            return RaceProblemsRepo.GetTests().Last();
        }

        [Test]
        public void CompareGreedyVariations()
        {
            var report = new StringBuilder();
            var random = new Random(123123);
            var problems = RaceProblemsRepo.Generate(10, 50, 5, random).ToList();
            var heuristicsSum = RunAi(
                problems,
                () => new HeuristicsRacer());

            for (int depth = 1; depth < 20; depth++)
            {
                report.Append(depth + "\t" + heuristicsSum + "\t");
                foreach (var extendWithNop in new[] { true, false })
                    foreach (var aggregation in new[] { Aggregation.Last, Aggregation.Max, Aggregation.Sum })
                    {
                        var d = depth;
                        var sum = RunAi(
                            problems, 
                            () => new GreedyRacer_Universal(new RaceEstimator(aggregation), d, extendWithNop));
                        report.Append(sum + "\t");
                    }
                report.Append("\n");
            }

            Console.WriteLine(report);
        }

        private static double RunAi(List<RaceState> problems, Func<ISolver<RaceState, RaceSolution>> factory)
        {
            var sum = 0.0;
            //for (int i = 0; i < 5; i++)
            foreach (var problem in problems)
            {
                var racer = factory();
                var time = RaceController.Play(problem.MakeCopy(), racer, false).Time;
                sum += time;
                //report.Append(time + "\t");
            }

            return sum;
        }


    }
}