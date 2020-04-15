using AiAlgorithms.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiAlgorithms.racing
{
    class RandomRacerWithoutMemorizing : ISolver<RaceState, RaceSolution>
    {
        private int Depth;
        private static ICarCommand[] Commands;
        private EvaluationFunctions EvaluationFunctions;
        static RandomRacerWithoutMemorizing()
        {
            var list = new[] { 0, 1, -1 };
            Commands = list
                .SelectMany(t => list, (t1, t2) => (ICarCommand)new MoveCommand(new V(t1, t2)))
                .Prepend((ICarCommand)new ExchangeCommand())
                .ToArray();
        }

        public RandomRacerWithoutMemorizing(int depth = 20, double flagsTakenc = 10000, double distc = 1, double nextFlagc = 1 / 4)
        {
            Depth = depth;
            EvaluationFunctions = new EvaluationFunctions(flagsTakenc, distc, nextFlagc);
        }

        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var ch = new SimpleConsistentFlagChooser();
            var pairOfFlags = ch.GetNextFlagsFor(problem);
            var firstCarRes = ChooseMoveForCar(true, problem, pairOfFlags.FirstCarNextFlag, ch);
            var secondCarRes = ChooseMoveForCar(false, problem, pairOfFlags.SecondCarNextFlag, ch);
            yield return new RaceSolution(new[]
            {(firstCarRes,secondCarRes)});
        }

        private ICarCommand ChooseMoveForCar(bool ifFirstCar, RaceState problem, V thisFlag, IFlagChooser chooser)
        {
            Random rnd = new Random();
            var resList = new List<(List<ICarCommand>, double, RaceState)>();
            for (int i = 0; i < 10; i++)
            {
                RaceState state = problem.MakeCopy();
                int allCount = 0;
                var evList = new List<double>();
                List<ICarCommand> myCommands = new List<ICarCommand>();
                while (allCount < Depth)
                {
                    var pairInd = rnd.Next(10);
                    var count = rnd.Next(3, 10);
                    var command = Commands[pairInd];
                    myCommands.Add(command);
                    allCount += count;
                    for (int j = 0; j < count; j++)
                        evList.Add(EvaluationFunctions.EvaluateCommand(state, ifFirstCar, thisFlag, command));
                }
                resList.Add((myCommands, evList.Max(), state));
            }
            var res_V = resList.OrderByDescending(pair => pair.Item2).First();
            return res_V.Item1.First();
        }
    }
}
