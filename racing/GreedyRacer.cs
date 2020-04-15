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
            var f = flags[state.FlagsTaken % flags.Count];
            var s = flags[(state.FlagsTaken + 1) % flags.Count];
            return (f, s);
        }
    }

    public class EvaluationFunctions
    {
        private double flagsTakenCoeff;
        private double distToFlagCoeff;
        private double bonusToNextFlagCoef;

        public EvaluationFunctions(double flagsTakenc = 10000, double distc =1, double nextFlagc=1/4)
        {
            flagsTakenCoeff = flagsTakenc;
            distToFlagCoeff = distc;
            bonusToNextFlagCoef = nextFlagc;
        }

        public double EvaluateExchange(RaceState state,
            bool ifFirstCar, V thisFlag)
        {
            return EvaluateCommand(state, ifFirstCar, thisFlag, new ExchangeCommand());
        }

        public void EvaluateMove(RaceState state,
            List<double> evList, V acceleration, bool ifFirstCar, V thisFlag)
        {
            evList.Add(EvaluateCommand(state, ifFirstCar, thisFlag,
                (ICarCommand)new MoveCommand(acceleration)));
        }

        public double EvaluateCommand(RaceState state,
            bool ifFirstCar, V thisFlag, ICarCommand command)
        {
            var car = ifFirstCar ? state.FirstCar : state.SecondCar;
            car.NextCommand = command;
            state.Tick();
            if (!car.IsAlive)
                return double.MinValue;
            else
            {
                var nextFlag = state.GetNextFlag();
                var bonusToNextFlag = (car.Pos + car.V).DistTo(nextFlag) - car.Pos.DistTo(nextFlag);
                var evaluation =
                    flagsTakenCoeff * car.FlagsTaken
                    - distToFlagCoeff * thisFlag.DistTo(car.Pos)
                    - bonusToNextFlagCoef * bonusToNextFlag;
                return evaluation;
            }
        }
    }

    public class GreedyRacer : ISolver<RaceState, RaceSolution>
    {
        private int Depth;
        private static List<V> Directions;
        private EvaluationFunctions EvaluationFunctions;

        static GreedyRacer()
        {
            var list = new[] { 0, 1, -1 };
            Directions = list
                .SelectMany(t => list, (t1, t2) => new V(t1, t2))
                .ToList();
        }

        public GreedyRacer(int depth = 20, double flagsTakenc = 10000, double distc = 1, double nextFlagc=1/4)
        {
            Depth = depth;
            EvaluationFunctions = new EvaluationFunctions(flagsTakenc, distc, nextFlagc);
        }

        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var ch = new SimpleConsistentFlagChooser();
            var pairOfFlags = ch.GetNextFlagsFor(problem);
            //можно выбирать в choosemove, но тада может сходить только 1
            var firstCarRes = ChooseMoveForCar(true, problem, pairOfFlags.FirstCarNextFlag, ch);
            var secondCarRes = ChooseMoveForCar(false, problem, pairOfFlags.SecondCarNextFlag, ch);
            var exchFisrt = double.NegativeInfinity;
            var exchSecond = double.NegativeInfinity;
            if (problem.ExchangeCooldown <= 0)//ващпе енто уже проверяется в тике
            {
                exchFisrt = EvaluationFunctions
                    .EvaluateExchange(problem,true,pairOfFlags.FirstCarNextFlag);
                exchSecond = EvaluationFunctions
                    .EvaluateExchange(problem, false, pairOfFlags.SecondCarNextFlag);
            }
            if (exchFisrt + exchSecond > firstCarRes.Item1 + secondCarRes.Item1)
                yield return new RaceSolution(new[] {((ICarCommand)new ExchangeCommand(),
                    (ICarCommand)new ExchangeCommand())});
            else
            yield return new RaceSolution(new[] 
            {((ICarCommand)new MoveCommand(firstCarRes.Item2),
                (ICarCommand)new MoveCommand(secondCarRes.Item2))});
        }

        private (double,V) ChooseMoveForCar(bool ifFirstCar, RaceState problem, V thisFlag, IFlagChooser chooser)
        {
            var dict = Directions.ToDictionary(pair => pair, pair => new List<double>());
            foreach (var pair in dict)
            {
                RaceState state = problem.MakeCopy();
                EvaluationFunctions
                    .EvaluateMove(state, pair.Value, pair.Key, ifFirstCar, thisFlag);
                for (int i = 0; i < Depth; i++)
                    EvaluationFunctions
                        .EvaluateMove(state, pair.Value, V.Zero, ifFirstCar, thisFlag);
            }
            var res_V = dict.OrderByDescending(pair => pair.Value.Max()).First();
            return (res_V.Value.Max(), res_V.Key);
        }
        public static double EvaluateExchange(RaceState state,
            bool ifFirstCar, V thisFlag, IFlagChooser chooser)
        {
            return EvaluateCommand(state,ifFirstCar,thisFlag, new ExchangeCommand());
        }

        public static void EvaluateMove(RaceState state, 
            List<double> evList, V acceleration, bool ifFirstCar, V thisFlag, IFlagChooser chooser)
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
            var evaluation =
                10000 * state.FlagsTaken
                - thisFlag.DistTo(car.Pos);
            return evaluation;
        }
    }
}