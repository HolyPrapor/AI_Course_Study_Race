using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    internal class RandomRacerWithoutMemorizing : ISolver<RaceState, RaceSolution>
    {
        private static readonly ICarCommand[] Commands;
        private readonly int depth;
        private readonly EvaluationFunctions evaluationFunctions;

        static RandomRacerWithoutMemorizing()
        {
            var list = new[] {0, 1, -1};
            Commands = list
                .SelectMany(t => list, (t1, t2) => (ICarCommand) new MoveCommand(new V(t1, t2)))
                .Prepend(new ExchangeCommand())
                .ToArray();
        }

        public RandomRacerWithoutMemorizing(int depth = 20, double flagsTakenC = 10000, double distC = 1,
            double nextFlagC = 1d / 4)
        {
            this.depth = depth;
            evaluationFunctions = new EvaluationFunctions(flagsTakenC, distC, nextFlagC);
        }

        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var ch = new SimpleConsistentFlagChooser();
            var pairOfFlags = ch.GetNextFlagsFor(problem);
            var firstCarRes = ChooseMoveForCar(true, problem, pairOfFlags.FirstCarNextFlag, ch);
            var secondCarRes = ChooseMoveForCar(false, problem, pairOfFlags.SecondCarNextFlag, ch);
            yield return new RaceSolution(new[]
                {(firstCarRes, secondCarRes)});
        }

        private ICarCommand ChooseMoveForCar(bool ifFirstCar, RaceState problem, V thisFlag, IFlagChooser chooser)
        {
            var rnd = new Random();
            var resList = new List<(List<ICarCommand>, double, RaceState)>();
            for (var i = 0; i < 10; i++)
            {
                var state = problem.MakeCopy();
                var allCount = 0;
                var evList = new List<double>();
                var myCommands = new List<ICarCommand>();
                while (allCount < depth)
                {
                    var pairInd = rnd.Next(10);
                    var count = rnd.Next(3, 10);
                    var command = Commands[pairInd];
                    myCommands.Add(command);
                    allCount += count;
                    for (var j = 0; j < count; j++)
                        evList.Add(evaluationFunctions.EvaluateCommand(state, ifFirstCar, thisFlag, command));
                }

                resList.Add((myCommands, evList.Max(), state));
            }

            var res_V = resList.OrderByDescending(pair => pair.Item2).First();
            return res_V.Item1.First();
        }
    }
}