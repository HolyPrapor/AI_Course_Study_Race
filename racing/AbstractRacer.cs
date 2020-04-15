using System.Collections.Generic;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public abstract class AbstractRacer : ISolver<RaceState, RaceSolution>
    {
        protected IFlagChooser FlagChooser;
        protected IMoveChooser MoveChooser;

        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var (firstCarFlag, secondCarFlag) = FlagChooser.GetNextFlagsFor(problem);
            var moves = MoveChooser.GetCarCommands(firstCarFlag, secondCarFlag,
                problem, out var debugInfo);
            yield return new RaceSolution(moves) {Debug = debugInfo};
        }
    }
}