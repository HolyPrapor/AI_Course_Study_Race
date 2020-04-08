using System;
using NUnit.Framework;

namespace AiAlgorithms.Trucks
{
    [TestFixture]
    public class HillClimbing_Tests : IScoredTest
    {
        [Test]
        public void QualityIsOk()
        {
            var totalScore = CalculateScore();
            Console.WriteLine($"Total score is {totalScore}");
            Assert.That(totalScore, Is.GreaterThan(MinScoreToPassTest));
        }

        public double CalculateScore()
        {
            var solver = new TrucksHillClimbingSolver();
            return TrucksSolverEvaluator.GetTotalScore(solver, 500, true);
        }

        public double MinScoreToPassTest => -10;
    }
}