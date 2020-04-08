using System;
using NUnit.Framework;

namespace AiAlgorithms.Trucks
{
    [TestFixture]
    public class GreedyTrucksSolver_Tests : IScoredTest
    {
        [Test]
        public void QualityIsOk()
        {
            var totalScore = CalculateScore();
            Assert.That(totalScore, Is.GreaterThan(MinScoreToPassTest));
        }

        public double CalculateScore()
        {
            var solver = new GreedyTrucksSolver();
            return TrucksSolverEvaluator.GetTotalScore(solver, 100, true);
        }

        public double MinScoreToPassTest => -150;

    }
}