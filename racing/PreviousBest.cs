using System.Collections.Generic;

namespace AiAlgorithms.racing
{
    public class PreviousBest
    {
        public readonly List<ICarCommand> CommandList;
        public readonly RaceState State;
        public double Score;

        public PreviousBest(List<ICarCommand> list, double sc, RaceState st)
        {
            CommandList = list;
            Score = sc;
            State = st;
        }
    }
}