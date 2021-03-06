using System;
using System.Collections.Generic;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class NaiveRacer : ISolver<RaceState, RaceSolution>
    {
        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var fChooser = new SimpleConsistentFlagChooser();
            var (f1, f2) = fChooser.GetNextFlagsFor(problem);
            var firstDelta = f1 - problem.FirstCar.Pos;
            var secondDelta = f2 - problem.SecondCar.Pos;
            yield return new RaceSolution(new (
                ICarCommand firstCarAcceleration,
                ICarCommand secondCarAcceleration)[]
                {
                    (new MoveCommand(new V(Math.Sign(firstDelta.X), Math.Sign(firstDelta.Y))),
                        new MoveCommand(new V(Math.Sign(secondDelta.X), Math.Sign(secondDelta.Y))))
                });
        }
    }
}