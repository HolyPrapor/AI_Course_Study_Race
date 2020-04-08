using System;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.Trucks
{
    public class TrucksHillClimbingSolver : HillClimbing<TrucksProblem, TrucksSolution>
    {
        public TrucksHillClimbingSolver()
            : base(null, null)
        {
        }
    }
}