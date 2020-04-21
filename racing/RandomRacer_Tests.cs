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

        [Test]
        public void PlayOneTestManyTimes()
        {
            var tests = RaceProblemsRepo.GetTests();
            var test = tests.ElementAt(6);
            int count = 40;
            var stat = new StatValue();
            var racer = new GreedyRacer();
            for (int i = 0; i < count; i++)
            {
                var finalState = RaceController.Play(test, racer, false);
                var testScore = finalState.FlagsTaken * 100 - finalState.Time;
                stat.Add(testScore);
            }
            var resWith = stat.Mean;
            Console.WriteLine("mean " + resWith.ToString() );
            Console.WriteLine("conf " + stat.ConfIntervalSize.ToString());
        }
    }
}