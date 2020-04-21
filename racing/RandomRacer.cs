﻿namespace AiAlgorithms.racing
{
    public class RandomRacer : AbstractRacer
    {
        public RandomRacer(int depth = 20,
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 1d / 4)
        {
            FlagChooser = new SimpleConsistentFlagChooser();
            MoveChooser = new RandomMoveChooser(new SumWeighter(), depth, flagsTakenC, distC, nextFlagC);
        }
    }
}