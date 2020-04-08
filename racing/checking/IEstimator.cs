namespace AiAlgorithms.racing.checking
{
    public interface IEstimator
    {
        double Estimate(RaceState problem, RaceSolution solution);
    }
}