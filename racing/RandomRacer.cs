using AiAlgorithms.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiAlgorithms.racing
{
    public class PreviousBest
    {
        public List<ICarCommand> commandList;
        public double score;
        public RaceState state;
        public int carNumber;

        public PreviousBest(int carN, List<ICarCommand> list, double sc, RaceState st)
        {
            carNumber = carN;
            commandList = list;
            score = sc;
            state = st;
        }
    }

    public class RandomRacer : ISolver<RaceState, RaceSolution>
    {
        private const int Depth = 40;
        private static ICarCommand[] Commands;
        public PreviousBest firstPreviousBest =null;
        public PreviousBest secondPreviousBest = null;
        static RandomRacer()
        {
            var list = new[] { 0, 1, -1 };
            Commands = list
                .SelectMany(t => list, (t1, t2) => (ICarCommand)new MoveCommand(new V(t1, t2)))
                .Prepend((ICarCommand)new ExchangeCommand())
                .ToArray();
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
                        evList.Add(GreedyRacer.EvaluateCommand(state,ifFirstCar,thisFlag,command));
                }
                resList.Add((myCommands, evList.Max(), state));
            }
            var prevBest = ifFirstCar ? firstPreviousBest : secondPreviousBest;
            if (prevBest != null)
            {
                var bestPairInd = rnd.Next(10);
                var addingCommand = Commands[bestPairInd];
                prevBest.commandList.Add(addingCommand);
                prevBest.score += GreedyRacer.EvaluateCommand(prevBest.state,
                    ifFirstCar, thisFlag,addingCommand);
                resList.Add((prevBest.commandList, prevBest.score, prevBest.state));
            }
            var res_V = resList.OrderByDescending(pair => pair.Item2).First();
            var bestList = res_V.Item1.Skip(1).ToList();
            prevBest = new PreviousBest(ifFirstCar?1:2,bestList, res_V.Item2, res_V.Item3);
            return res_V.Item1.First();
        }
    }
}
