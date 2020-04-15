using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class RandomRacer : ISolver<RaceState, RaceSolution>
    {
        private static readonly ICarCommand[] Commands;
        private readonly int depth;
        private readonly EvaluationFunctions evaluationFunctions;
        private PreviousBest firstPreviousBest = null;
        private PreviousBest secondPreviousBest = null;

        static RandomRacer()
        {
            var list = new[] {0, 1, -1};
            Commands = list
                .SelectMany(t => list, (t1, t2) => (ICarCommand) new MoveCommand(new V(t1, t2)))
                .Prepend(new ExchangeCommand())
                .ToArray();
        }

        public RandomRacer(int depth = 20, double flagsTakenC = 10000, double distC = 1, double nextFlagC = 1 / 4)
        {
            this.depth = depth;
            evaluationFunctions = new EvaluationFunctions(flagsTakenC, distC, nextFlagC);
        }

        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var chooser = new SimpleConsistentFlagChooser();
            var (firstCarNextFlag, secondCarNextFlag) = chooser.GetNextFlagsFor(problem);
            var firstCarResult = ChooseMoveForCar(true, problem, firstCarNextFlag);
            var secondCarResult = ChooseMoveForCar(false, problem, secondCarNextFlag);
            yield return new RaceSolution(new[]
                {(firstCarResult, secondCarResult)});
        }

        private ICarCommand ChooseMoveForCar(bool ifFirstCar, RaceState problem, V thisFlag)
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

            var prevBest = ifFirstCar ? firstPreviousBest : secondPreviousBest;
            if (prevBest != null)
                foreach (var addingCommand in Commands)
                {
                    var addedList = new List<ICarCommand>(prevBest.CommandList) {addingCommand};
                    prevBest.Score += evaluationFunctions.EvaluateCommand(prevBest.State,
                        ifFirstCar, thisFlag, addingCommand);
                    resList.Add((prevBest.CommandList, prevBest.Score, prevBest.State));
                }

            var res_V = resList.OrderByDescending(pair => pair.Item2).First();
            var bestList = res_V.Item1.Skip(1).ToList();
            prevBest = new PreviousBest(bestList, res_V.Item2, res_V.Item3);
            return res_V.Item1.First();
        }
    }
}