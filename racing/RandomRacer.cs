namespace AiAlgorithms.racing
{
    public class RandomRacer : AbstractRacer
    {
        public RandomRacer(int depth = 20,
<<<<<<< HEAD
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 0.25)
        {
            FlagChooser = new MaxDistFlagChooser();
            MoveChooser = new RandomMoveChooser(new SumWeighter(), depth, flagsTakenC, distC, nextFlagC);
=======
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 1d / 4)
        {
            FlagChooser = new MaxDistFlagChooser();
            MoveChooser = new MixedMoveChooser();
            //MoveChooser = new RandomMoveChooser(new SumWeighter(), depth, flagsTakenC, distC, nextFlagC);
>>>>>>> 7f0cf6de205f5c2f8f9507b505d5f66729da7683
        }
    }
}