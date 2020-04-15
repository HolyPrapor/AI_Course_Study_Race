using System;
using System.Linq;
using NUnit.Framework;

namespace AiAlgorithms.racing
{
    [TestFixture]
    public class RandomRacer_Tests : Base_Tests, IScoredTest
    {
        public RandomRacer_Tests():base((AbstractRacer)new RandomRacer())
        {}
    }
}