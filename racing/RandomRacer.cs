namespace AiAlgorithms.racing
{
    public class RandomRacer : AbstractRacer
    {
        public RandomRacer(int depth = 20,
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 0.25)
        {
            FlagChooser = new MaxDistFlagChooser();
            MoveChooser = new RandomMoveChooser(new SumWeighter(), depth, flagsTakenC, distC, nextFlagC);
        }
    }
}