using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class RaceState
    {
        private readonly IReadOnlyCollection<Car> cars;
        public readonly Car FirstCar;
        public readonly Car SecondCar;
        public readonly RaceTrack Track;

        public RaceState(RaceTrack track, Car firstCar, Car secondCar)
        {
            Track = track;
            FirstCar = firstCar;
            SecondCar = secondCar;
            cars = new List<Car> {FirstCar, SecondCar};
        }

        public int FlagsTaken { get; private set; }
        public int ExchangeCooldown { get; private set; }

        public int Time { get; private set; }
        public bool IsFinished => Time >= Track.RaceDuration || FlagsTaken >= Track.FlagsToTake;

        public RaceState MakeCopy()
        {
            return new RaceState(Track, FirstCar.MakeCopy(), SecondCar.MakeCopy())
            {
                Time = Time,
                ExchangeCooldown = ExchangeCooldown, FlagsTaken = FlagsTaken
            };
        }

        public void Tick()
        {
            if (IsFinished) return;
            if (FirstCar.NextCommand is ExchangeCommand && SecondCar.NextCommand is ExchangeCommand &&
                ExchangeCooldown <= 0)
            {
                var temp = FirstCar.V;
                FirstCar.V = SecondCar.V;
                SecondCar.V = temp;
                ExchangeCooldown = 20;
            }

            foreach (var car in cars)
                if (car.IsAlive)
                {
                    var initialPos = car.Pos;
                    car.Tick();
                    var finalPos = car.Pos;
                    if (CrashToObstacle(initialPos, finalPos, car.Radius))
                        car.IsAlive = false;
                    else
                        while (SegmentCrossPoint(initialPos, finalPos, GetNextFlag(), car.Radius)
                            && FlagsTaken<Track.FlagsToTake)
                        {
                            FlagsTaken++;
                            car.FlagsTaken++;
                        }
                }

            Time++;
            ExchangeCooldown--;
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

        public V GetNextFlag(int offset = 0)
        {
            return Track.Flags[(FlagsTaken + offset) % Track.Flags.Count];
        }

        public override string ToString()
        {
            return $"FirstCar: {FirstCar}, SecondCar: {SecondCar}, Time: {Time}, IsFinished: {IsFinished}";
        }
    }
}