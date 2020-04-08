using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing.checking
{
    public class HeuristicsRacer : ISolver<RaceState, RaceSolution>
    {
        public RaceSolution GetDeepSolution(RaceState state, int depth)
        {
            var copy = state.MakeCopy();
            var accelerations = new List<V>();
            for (int i = 0; i < depth; i++)
            {
                var acceleration = GetSolutions(copy, 100).Last().Accelerations.Single();
                accelerations.Add(acceleration);
                copy.Car.NextCommand = acceleration;
                copy.Tick();
            }
            return new RaceSolution(accelerations.ToArray());
        }

        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var flag = problem.GetFlagFor(problem.Car);
            var carV = problem.Car.V;
            var target = flag - new V(carV.X * Math.Abs(carV.X+1), carV.Y* Math.Abs(carV.Y+1))*15/20;
            //var target = flag - 5 * carV;
            var delta = target - problem.Car.Pos;
            var acceleration = new V(Math.Sign(delta.X), Math.Sign(delta.Y));
            yield return new RaceSolution(new[] {acceleration});
        }
    }
}