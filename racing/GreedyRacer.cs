using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class SimpleChooser : IFlagChooser
    {
        public (V FirstCarNextFlag, V SecondCarNextFlag) GetNextFlagsFor(RaceState state)
        {
            var flags = state.Track.Flags;
            var firstCar = state.FirstCar;
            var secondCar = state.SecondCar;
            var bothTakenCount = firstCar.FlagsTaken + secondCar.FlagsTaken;
            var f = flags[(bothTakenCount) % flags.Count];
            var s = flags[(bothTakenCount + 1) % flags.Count];
            return (f, s);
        }
    }

    public class GreedyRacer : ISolver<RaceState, RaceSolution>
    {
        private const int Depth=20;
        private static List<V> Directions;

        static GreedyRacer()
        {
            var list = new[] { 0, 1, -1 };
            Directions = list
                .SelectMany(t => list, (t1, t2) => new V(t1, t2))
                .ToList();
        }

        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var ch = new SimpleChooser();
            var pairOfFlags = ch.GetNextFlagsFor(problem);
            //можно выбирать в choosemove, но тада может сходить только 1
            var firstCarRes = ChooseMoveForCar(true, problem, pairOfFlags.FirstCarNextFlag);
            var secondCarRes = ChooseMoveForCar(false, problem, pairOfFlags.SecondCarNextFlag);
            var exchFisrt = double.NegativeInfinity;
            var exchSecond = double.NegativeInfinity;
            if (problem.ExchangeCooldown <= 0)//ващпе енто уже проверяется в тике
            {
                exchFisrt = EvaluateExchange(problem,true,pairOfFlags.FirstCarNextFlag);
                exchSecond = EvaluateExchange(problem, false, pairOfFlags.SecondCarNextFlag);
            }
            if (exchFisrt + exchSecond > firstCarRes.Item1 + secondCarRes.Item1)
                yield return new RaceSolution(new[] {((ICarCommand)new ExchangeCommand(),
                    (ICarCommand)new ExchangeCommand())});
            else
            yield return new RaceSolution(new[] 
            {((ICarCommand)new MoveCommand(firstCarRes.Item2),
                (ICarCommand)new MoveCommand(secondCarRes.Item2))});
        }

        private (double,V) ChooseMoveForCar(bool ifFirstCar, RaceState problem, V thisFlag)
        {
            var dict = Directions.ToDictionary(pair => pair, pair => new List<double>());
            foreach (var pair in dict)
            {
                RaceState state = problem.MakeCopy();
                EvaluateMove(state, pair.Value, pair.Key, ifFirstCar, thisFlag);
                for (int i = 0; i < Depth; i++)
                    EvaluateMove(state, pair.Value, V.Zero, ifFirstCar, thisFlag);
            }
            var res_V = dict.OrderByDescending(pair => pair.Value.Max()).First();
            return (res_V.Value.Max(), res_V.Key);
        }

        public static double EvaluateExchange(RaceState state,
            bool ifFirstCar, V thisFlag)
        {
            return EvaluateCommand(state,ifFirstCar,thisFlag, new ExchangeCommand());
        }

        public static void EvaluateMove(RaceState state, 
            List<double> evList, V acceleration, bool ifFirstCar, V thisFlag)
        {
            evList.Add(EvaluateCommand(state, ifFirstCar, thisFlag,
                (ICarCommand)new MoveCommand(acceleration)));
        }

        public static double EvaluateCommand(RaceState state,
            bool ifFirstCar, V thisFlag, ICarCommand command)
        {
            var car = ifFirstCar ? state.FirstCar : state.SecondCar;
            car.NextCommand = command;
            state.Tick();
            if (!car.IsAlive)
                return double.MinValue;
            else
            {
                var evaluation =
                    10000 * car.FlagsTaken
                    - thisFlag.DistTo(car.Pos);
                 return evaluation;
            }
        }
    }
}