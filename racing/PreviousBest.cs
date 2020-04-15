using System.Collections.Generic;

namespace AiAlgorithms.racing
{
    public class PreviousBest
    {
        public readonly List<ICarCommand> CommandList;
        public double Score;
        public readonly RaceState State;

        public PreviousBest(List<ICarCommand> list, double sc, RaceState st)
        {
            CommandList = list;
            Score = sc;
            State = st;
        }
    }
}