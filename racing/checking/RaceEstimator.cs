using System;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithms.racing.checking
{
    public class RaceEstimator : IEstimator
    {
        private readonly Aggregation aggregation;

        public RaceEstimator(Aggregation aggregation)
        {
            this.aggregation = aggregation;
        }

        public double Estimate(RaceState state, RaceSolution solution)
        {
            var states = Simulate(state, solution);
            switch (aggregation)
            {
                case Aggregation.Max: return states.Max(EstimateState);
                case Aggregation.Last: return states.Select(EstimateState).Last();
                case Aggregation.Sum: return states.Sum(EstimateState);
                default: throw new Exception(aggregation.ToString());
            }
        }

        private static double EstimateState(RaceState state)
        {
            var aliveBonus = state.Car.IsAlive ? 1 : 0;
            var distToNextFlagPenalty = Math.Sqrt(state.Car.Pos.Dist2To(state.GetFlagFor(state.Car)));
            return
                state.Car.FlagsTaken * 1000
                + aliveBonus * 10000
                - distToNextFlagPenalty
                -state.Time*0.7
                ;
        }

        private IEnumerable<RaceState> Simulate(RaceState problem, RaceSolution solution)
        {
            return new RaceController().Play(problem, solution);
        }
    }
}