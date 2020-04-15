using System.Collections.Generic;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class RaceTrack
    {
        public readonly IReadOnlyList<V> Flags;
        public readonly int FlagsToTake;
        public readonly IReadOnlyList<Disk> Obstacles;
        public readonly int RaceDuration;

        public RaceTrack(IReadOnlyList<V> flags, IReadOnlyList<Disk> obstacles, int raceDuration, int flagsToTake)
        {
            Flags = flags;
            Obstacles = obstacles;
            RaceDuration = raceDuration;
            FlagsToTake = flagsToTake;
        }
    }
}