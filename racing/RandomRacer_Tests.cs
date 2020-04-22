using System;
using System.Linq;
using AiAlgorithms.Algorithms;
using NUnit.Framework;

namespace AiAlgorithms.racing
{
    [TestFixture]
    public class RandomRacer_Tests : Base_Tests, IScoredTest
    {
        public RandomRacer_Tests():base((AbstractRacer)new RandomRacer(true))
        {}

        //[Test]
        public void PlayOneTestManyTimes(int testNumber, int repetitionCount)
        {
            var tests = RaceProblemsRepo.GetTests();
            var test = tests.ElementAt(testNumber);
            var stat = new StatValue();
            var racer = new DoubleRandomRacer();
            for (int i = 0; i < repetitionCount; i++)
            {
                var finalState = RaceController.Play(test, racer, false);
                var testScore = finalState.FlagsTaken * 100 - finalState.Time;
                stat.Add(testScore);
            }
            var resWith = stat.Mean;
            Console.WriteLine(testNumber.ToString());
            Console.WriteLine("mean " + resWith.ToString());
            Console.WriteLine("conf " + stat.ConfIntervalSize.ToString());
        }

        [Test]
        public void MeanAndConfForAll()
        {

            var count = RaceProblemsRepo.GetTests().Count();
            for (int i = 0; i < count; i++)
            {
                PlayOneTestManyTimes(i, 20);
            }

            var totalScore = CalculateScore();
            Console.WriteLine($"Total score is {totalScore}");
            Assert.That(totalScore, Is.GreaterThan(MinScoreToPassTest));
        }
    }
}