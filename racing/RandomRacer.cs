using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class RandomRacer : AbstractRacer
    {
        public RandomRacer(int depth = 20,
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 1 / 4)
        {
            FlagChooser = new MaxDistFlagChooser();
            MoveChooser = new RandomMoveChooser(new SumWeighter(),depth, flagsTakenC, distC, nextFlagC);
        }
    }
}