using System;
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

        public double CalculateScore()
        {
            var score = 0.0;
            int iTest = 0;
            foreach (var test in RaceProblemsRepo.GetTests())
            {
                var racer = new GreedyRacer();
                var finalState = RaceController.Play(test, racer, false);
                var testScore = finalState.FlagsTaken * 100 - finalState.Time;
                Console.WriteLine($"Test #{iTest} score: {testScore} (flags: {finalState.FlagsTaken} of {test.Track.FlagsToTake}, time: {finalState.Time} of {test.Track.RaceDuration})");
                score += testScore;
                iTest++;
            }
            return score;
        }
    }
}