namespace AiAlgorithms.racing
{
    public class RandomRacer : AbstractRacer
    {
        public RandomRacer(bool withExchange,int depth = 20,
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 0.25)
        {
            FlagChooser = new MaxDistFlagChooser();
            MoveChooser = new RandomMoveChooser(new SumWeighter(), 
                withExchange, depth, flagsTakenC, distC, nextFlagC);
            //FlagChooser = new DeepFlagChooser(MoveChooser);
        }
    }
}