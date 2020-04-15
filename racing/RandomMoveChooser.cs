using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class RandomMoveChooser : IMoveChooser
    {
        private static readonly ICarCommand[] Commands;
        private readonly int depth;
        private readonly EvaluationFunctions evaluationFunctions;
        private List<ICarCommand> firstPreviousBest = null;
        private List<ICarCommand> secondPreviousBest = null;
        private readonly IPairWeighter PairWeighter;

        static RandomMoveChooser()
        {
            var list = new[] {0, 1, -1};
            Commands = list
                .SelectMany(t => list, (t1, t2) => (ICarCommand) new MoveCommand(new V(t1, t2)))
                .Prepend(new ExchangeCommand())
                .ToArray();
        }

        public RandomMoveChooser(IPairWeighter pairWeight ,int depth = 20, 
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 1 / 4)
        {
            this.depth = depth;
            evaluationFunctions = new EvaluationFunctions(flagsTakenC, distC, nextFlagC);
            PairWeighter = pairWeight;
        }
        
        private (ICarCommand, double) ChooseMoveForCar(bool ifFirstCar, RaceState problem, V thisFlag)
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
            {
                foreach (var addingCommand in Commands)
                {
                    var addedList = new List<ICarCommand>(prevBest) { addingCommand };
                    var newState = problem.MakeCopy();
                    var scoreList = new List<double>();
                    foreach (var com in addedList)
                        scoreList.Add(evaluationFunctions.EvaluateCommand(newState, ifFirstCar, thisFlag, com));
                    var newScore = scoreList.Max();
                    resList.Add((addedList, newScore, newState));
                }
            }
            var res_V = resList.OrderByDescending(pair => pair.Item2).First();
            var bestList = res_V.Item1.Skip(1).ToList();
            if (ifFirstCar)
                firstPreviousBest = bestList;
            else
                secondPreviousBest = bestList;
            return (res_V.Item1.First(), res_V.Item2);
        }
        
        public (ICarCommand FirstCarCommand, ICarCommand SecondCarCommand, double Score)[] GetCarCommands(V nextFlagForFirstCar,
            V nextFlagForSecondCar, RaceState raceState, out string debugInfo)
        {
            debugInfo = "";
            var firstMoveAndScore = ChooseMoveForCar(true, raceState, nextFlagForFirstCar);
            var secondMoveAndScore = ChooseMoveForCar(false, raceState, nextFlagForSecondCar);
            return new[]
            {
                (firstMoveAndScore.Item1,secondMoveAndScore.Item1,
                PairWeighter.WeightPair(firstMoveAndScore.Item2, secondMoveAndScore.Item2))
            };
        }
    }
}