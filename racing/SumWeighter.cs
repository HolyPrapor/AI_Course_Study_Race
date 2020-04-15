namespace AiAlgorithms.racing
{
    public class SumWeighter : IPairWeighter
    {
        public double WeightPair(double firstElement, double secondElement)
        {
            return firstElement + secondElement;
        }
    }
}