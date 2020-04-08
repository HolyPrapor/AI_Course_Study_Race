using System;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class RaceState
    {
        public readonly RaceTrack Track;
        public readonly Car Car;

        public RaceState(RaceTrack track, Car car)
        {
            Track = track;
            Car = car;
        }

        public int Time { get; private set; }
        public bool IsFinished => Time >= Track.RaceDuration || Car.FlagsTaken >= Track.FlagsToTake;

        public RaceState MakeCopy()
        {
            return new RaceState(Track, Car.MakeCopy()) {Time = Time};
        }

        public void Tick()
        {
            if (IsFinished) return;
            if (Car.IsAlive)
            {
                var initialPos = Car.Pos;
                Car.Tick();
                var finalPos = Car.Pos;
                if (CrashToObstacle(initialPos, finalPos, Car.Radius))
                    Car.IsAlive = false;
                else
                    while (SegmentCrossPoint(initialPos, finalPos, GetFlagFor(Car), Car.Radius))
                        Car.FlagsTaken++;
            }

            Time++;
        }

        private bool CrashToObstacle(V a, V b, int carRadius)
        {
            return Track.Obstacles.Any(o => SegmentCrossPoint(a, b, o.Pos, o.Radius + carRadius));
        }

        private bool SegmentCrossPoint(V a, V b, V point, int crossDistance)
        {
            return DistPointToSegment(point, a, b) <= crossDistance;
        }

        private double DistPointToSegment(V p, V a, V b)
        {
            var cos1 = (b - a) * (p - a);
            var cos2 = (a - b) * (p - b);
            if (cos1 <= 0) return p.DistTo(a);
            if (cos2 <= 0) return p.DistTo(b);
            return Math.Abs(((b - a) ^ (p - a)) / a.DistTo(b));
        }

        public V GetFlagFor(Car car)
        {
            return Track.Flags[car.FlagsTaken % Track.Flags.Count];
        }

        public override string ToString()
        {
            return $"Car: {Car}, Time: {Time}, IsFinished: {IsFinished}";
        }
    }
}