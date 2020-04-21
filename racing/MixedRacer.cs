using System;
using System.Collections.Generic;
using System.Text;

namespace AiAlgorithms.racing
{
    class MixedRacer : AbstractRacer
    {
        public MixedRacer(int depth = 20,
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 0.25)
        {
            FlagChooser = new MaxDistFlagChooser();
            MoveChooser = new MixedMoveChooser(depth, flagsTakenC, distC, nextFlagC);
        }
    }
}
