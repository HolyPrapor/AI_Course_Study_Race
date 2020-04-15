using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class SimpleChooser : IFlagChooser
    {
        public (V FirstCarNextFlag, V SecondCarNextFlag) GetNextFlagsFor(RaceState state)
        {
            var flags = state.Track.Flags;
            var f = flags[state.FlagsTaken % flags.Count];
            var s = flags[(state.FlagsTaken + 1) % flags.Count];
            return (f, s);
        }
    }
}