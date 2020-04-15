using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiAlgorithms.racing
{
    [TestFixture]
    class MixedRacer_Tests: Base_Tests, IScoredTest
    {
        public MixedRacer_Tests() : base((AbstractRacer)new MixedRacer())
        { }
    }
}
