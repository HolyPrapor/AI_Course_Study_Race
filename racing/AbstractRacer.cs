using System.Collections.Generic;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public abstract class AbstractRacer : ISolver<RaceState, RaceSolution>
    {
        protected readonly IMoveChooser MoveChooser;
        protected readonly IFlagChooser FlagChooser;
        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var (firstCarFlag, secondCarFlag) = FlagChooser.GetNextFlagsFor(problem);
            var moves = MoveChooser.GetCarCommands(firstCarFlag, secondCarFlag,
                problem);
            yield return new RaceSolution(moves);
        }
    }
}