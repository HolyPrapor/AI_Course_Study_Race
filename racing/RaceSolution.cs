using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class RaceSolution : ISolution
    {
        public readonly V[] Accelerations;

        public RaceSolution(V[] accelerations)
        {
            Accelerations = accelerations;
        }

        public override string ToString()
        {
            return $"Score: {Score}, Accelerations: {Accelerations.StrJoin(" ")}";
        }

        public double Score { get; set; }
    }
}