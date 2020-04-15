using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class GreedyRacer : AbstractRacer
    {
        public GreedyRacer(int depth = 20,
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 1 / 4)
        {
            FlagChooser = new MaxDistFlagChooser();
            MoveChooser = new GreedyMoveChooser(new SumWeighter(), depth, flagsTakenC, distC, nextFlagC);
        }
    }
}