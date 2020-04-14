using System;
using System.Linq;
using NUnit.Framework;

namespace AiAlgorithms.racing
{
    [TestFixture]
    public class RandomRacer_Tests : IScoredTest
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
        [Explicit("Тест для отладки и анализа")]
        public void VisualizeRace([Values(8)]int testIndex)
        {
            // Открой файл bin/Debug/*/racing/visualizer/index.html чтобы посмотреть реплей на тесте testIndex
            var greedyRacer = new RandomRacer();
            var test = RaceProblemsRepo.GetTests().ElementAt(testIndex);
            RaceController.Play(test, greedyRacer, true);
        }

        public double CalculateScore()
        {
            var score = 0.0;
            int iTest = 0;
            foreach (var test in RaceProblemsRepo.GetTests())
            {
                var racer = new RandomRacer();
                var finalState = RaceController.Play(test, racer, false);
                var testScore = finalState.FirstCar.FlagsTaken * 100 - finalState.Time;
                Console.WriteLine($"Test #{iTest} score: {testScore} (flags: {finalState.FirstCar.FlagsTaken} of {test.Track.FlagsToTake}, time: {finalState.Time} of {test.Track.RaceDuration})");
                score += testScore;
                iTest++;
            }
            return score;
        }

    }
}
