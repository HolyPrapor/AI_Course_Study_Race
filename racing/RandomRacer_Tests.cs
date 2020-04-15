using System;
using System.Linq;
using NUnit.Framework;

namespace AiAlgorithms.racing
{
    [TestFixture]
    public class RandomRacer_Tests : Base_Tests, IScoredTest
    {
<<<<<<< HEAD
        public RandomRacer_Tests():base((AbstractRacer)new RandomRacer())
        {}
=======
        public double MinScoreToPassTest => 8400;

        public double CalculateScore()
        {
            var score = 0.0;
            var iTest = 0;
            foreach (var test in RaceProblemsRepo.GetTests())
            {
                var racer = new RandomRacer();
                var finalState = RaceController.Play(test, racer, false);
                var testScore = finalState.FlagsTaken * 100 - finalState.Time;
                Console.WriteLine(
                    $"Test #{iTest} score: {testScore} (flags: {finalState.FlagsTaken} of {test.Track.FlagsToTake}, time: {finalState.Time} of {test.Track.RaceDuration})");
                score += testScore;
                iTest++;
            }

            return score;
        }

        [Test]
        public void QualityIsOK()
        {
            var totalScore = CalculateScore();
            Console.WriteLine($"Total score is {totalScore}");
            Assert.That(totalScore, Is.GreaterThan(MinScoreToPassTest));
        }

        [Test]
        [Explicit("Тест для отладки и анализа")]
        public void VisualizeRace([Values(1)] int testIndex)
        {
            // Открой файл bin/Debug/*/racing/visualizer/index.html чтобы посмотреть реплей на тесте testIndex
            var greedyRacer = new RandomRacer();
            var test = RaceProblemsRepo.GetTests().ElementAt(testIndex);
            RaceController.Play(test, greedyRacer, true);
        }
>>>>>>> 7f0cf6de205f5c2f8f9507b505d5f66729da7683
    }
}