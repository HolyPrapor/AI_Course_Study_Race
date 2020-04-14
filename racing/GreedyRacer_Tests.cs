using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AiAlgorithms.racing
{
    [TestFixture]
    public class GreedyRacer_Tests : IScoredTest
    {
        public double MinScoreToPassTest => 8400;

        [Test]
        public void QualityIsOK()
        {
            var totalScore = CalculateScore();
            Console.WriteLine($"Total score is {totalScore}");
            Assert.That(totalScore, Is.GreaterThan(MinScoreToPassTest));
        }

        [Test]
        [Explicit("���� ��� ������� � �������")]
        public void VisualizeRace([Values(2)]int testIndex)
        {
            // ������ ���� bin/Debug/*/racing/visualizer/index.html ����� ���������� ������ �� ����� testIndex
            var greedyRacer = new GreedyRacer();
            var test = RaceProblemsRepo.GetTests().ElementAt(testIndex);
            RaceController.Play(test, greedyRacer, true);
        }

        [Test]
        public void ChooseCoeffsRandom()
        {
            var res = new List<(int, int, int,double, double)>();
            for (int i = 0; i < 100; i ++)
            {
                var rnd = new Random();
                var depth = rnd.Next(5, 30);
                var flagsTaken = rnd.Next(1000, 10000);
                var distc = rnd.Next(1,10);
                var nextFlag = rnd.NextDouble();
                var score = CalculateScore1(depth, flagsTaken, distc, nextFlag);
                res.Add((depth,flagsTaken,distc, nextFlag,score));
            }
            var bestCoeff = res
                .OrderBy(p => p.Item5)
                .First();
            Console.WriteLine($"depth {bestCoeff.Item1} flags {bestCoeff.Item2}" +
                $" dist {bestCoeff.Item3} nextbonus {bestCoeff.Item4} score {bestCoeff.Item5}");
        }

        [Test]
        public void ChooseCoeffsBruteForce()
        {
            var res = new List<(int, int, int, double, double)>();
            var depth = Enumerable.Range(5, 30);
            var flagsTaken = Enumerable.Range(1000, 2000);
            var distc = Enumerable.Range(1, 10);
            var nextFlag = Enumerable.Range(0,5).Select(x=>x*0.5);
            var allCombinations = depth
                .SelectMany(d=>flagsTaken,(d,f)=>(d,f))
                .SelectMany(df=>distc, (df, dis)=>(df.d, df.f, dis))
                .SelectMany(dfdis=>nextFlag, (dfdis, n)=>(dfdis.d, dfdis.f, dfdis.dis,n));
            foreach (var tup in allCombinations.Take(500))
            {
                var score = CalculateScore1(tup.d, tup.f, tup.dis, tup.n);
                res.Add((tup.d, tup.f, tup.dis, tup.n, score));
            }
            var bestCoeff = res
                .OrderBy(p => p.Item5)
                .First();
            Console.WriteLine($"depth {bestCoeff.Item1} flags {bestCoeff.Item2}" +
                $" dist {bestCoeff.Item3} nextbonus {bestCoeff.Item4} score {bestCoeff.Item5}");
        }

        public double CalculateScore1(int depth = 20, double flagsTakenc = 10000,
            double distc = 1, double nextFlagc = 1 / 4)
        {
            var score = 0.0;
            int iTest = 0;
            foreach (var test in RaceProblemsRepo.GetTests())
            {
                var racer = new GreedyRacer(depth,flagsTakenc,distc,nextFlagc);
                var finalState = RaceController.Play(test, racer, false);
                var testScore = finalState.FlagsTaken * 100 - finalState.Time;
                Console.WriteLine($"Test #{iTest} score: {testScore} (flags: {finalState.FlagsTaken} of {test.Track.FlagsToTake}, time: {finalState.Time} of {test.Track.RaceDuration})");
                score += testScore;
                iTest++;
            }
            return score;
        }

        public double CalculateScore()
        {
            return CalculateScore1();
        }
    }
}