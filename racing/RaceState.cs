using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class RaceState
    {
        public readonly RaceTrack Track;
        public readonly Car FirstCar;
        public readonly Car SecondCar;
        private readonly IReadOnlyCollection<Car> cars;
        public int ExchangeCooldown { get; private set; } = 0;
        public int TotalTakenFlags => FirstCar.FlagsTaken + SecondCar.FlagsTaken;

        public RaceState(RaceTrack track, Car firstCar, Car secondCar)
        {
            Track = track;
            FirstCar = firstCar;
            SecondCar = secondCar;
            cars = new List<Car> {FirstCar, SecondCar};
        }

        public int Time { get; private set; }
        public bool IsFinished => Time >= Track.RaceDuration || FirstCar.FlagsTaken >= Track.FlagsToTake;

        public RaceState MakeCopy()
        {
            return new RaceState(Track, FirstCar.MakeCopy(), SecondCar.MakeCopy()) {Time = Time,
                ExchangeCooldown = ExchangeCooldown};
        }

        public void Tick(IFlagChooser chooser)
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
            {
                if (car.IsAlive)
                {
                    var initialPos = car.Pos;
                    car.Tick();
                    var finalPos = car.Pos;
                    if (CrashToObstacle(initialPos, finalPos, car.Radius))
                        car.IsAlive = false;
                    else
                    {
                        while (true)
                        {
                            var nextFlag = GetFlagFor(car, chooser);
                            if (nextFlag == GetNextFlag()
                                && SegmentCrossPoint(initialPos, finalPos, nextFlag, car.Radius))
                                car.FlagsTaken++;
                            else
                                break;
                        }
                    }
                }
            }
            Time++;
            ExchangeCooldown--;
        }

        public V GetNextFlag(int shift = 0)
        {
            return Track.Flags[(TotalTakenFlags + shift) % Track.Flags.Count];
        }

        private bool CrashToObstacle(V a, V b, int carRadius)
        {
            return Track.Obstacles.Any(o => SegmentCrossPoint(a, b, o.Pos, o.Radius + carRadius));
        }

        private bool SegmentCrossPoint(V a, V b, V point, int crossDistance)
        {
            return point!=null && DistPointToSegment(point, a, b) <= crossDistance;
        }

        private double DistPointToSegment(V p, V a, V b)
        {
            var cos1 = (b - a) * (p - a);
            var cos2 = (a - b) * (p - b);
            if (cos1 <= 0) return p.DistTo(a);
            if (cos2 <= 0) return p.DistTo(b);
            return Math.Abs(((b - a) ^ (p - a)) / a.DistTo(b));
        }

        public V GetFlagFor(Car car, IFlagChooser chooser)
        {
            var (f1, f2) = chooser.GetNextFlagsFor(this);

            if (car == FirstCar)
                return f1;

            return f2;
        }

        public override string ToString()
        {
            return $"FirstCar: {FirstCar}, SecondCar: {SecondCar}, Time: {Time}, IsFinished: {IsFinished}";
        }
    }
}