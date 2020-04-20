using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiAlgorithms.racing
{
    public abstract class Base_Tests:IScoredTest
    {
        public AbstractRacer Racer;
        public Base_Tests(AbstractRacer racer)
        {
            Racer = racer;
        }

        public double MinScoreToPassTest => 8400;

        public double CalculateScore()
        {
            var score = 0.0;
            var iTest = 0;
            foreach (var test in RaceProblemsRepo.GetTests())
            {
                var racer = Racer;
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
        public void VisualizeRace([Values(10)] int testIndex)
        {
            // Открой файл bin/Debug/*/racing/visualizer/index.html чтобы посмотреть реплей на тесте testIndex
            var greedyRacer = Racer;
            var test = RaceProblemsRepo.GetTests().ElementAt(testIndex);
            RaceController.Play(test, greedyRacer, true);
        }
    }
}
