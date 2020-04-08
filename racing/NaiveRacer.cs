using System;
using System.Collections.Generic;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class NaiveRacer : ISolver<RaceState, RaceSolution>
    {
        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var firstDelta = problem.GetFlagFor(problem.FirstCar) - problem.FirstCar.Pos;
            var secondDelta = problem.GetFlagFor(problem.SecondCar) - problem.SecondCar.Pos;
            yield return new RaceSolution(new (
                ICarCommand firstCarAcceleration,
                ICarCommand secondCarAcceleration)[]
                {(new MoveCommand(new V(Math.Sign(firstDelta.X), Math.Sign(firstDelta.Y))),
                    new MoveCommand(new V(Math.Sign(secondDelta.X), Math.Sign(secondDelta.Y))))});
        }
    }
}