using AiAlgorithms.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiAlgorithms.racing
{
    public class RandomRacer : ISolver<RaceState, RaceSolution>
    {
        private const int Depth = 40;
        private static V[] Directions;

        //здесь нет exchange
        static RandomRacer()
        {
            var list = new[] { 0, 1, -1 };
            Directions = list.SelectMany(t => list, (t1, t2) => new V(t1, t2)).ToArray();
        }

        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var ch = new SimpleChooser();
            var pairOfFlags = ch.GetNextFlagsFor(problem);
            var firstCarRes = ChooseMoveForCar(true, problem, pairOfFlags.FirstCarNextFlag);
            var secondCarRes = ChooseMoveForCar(false, problem, pairOfFlags.SecondCarNextFlag);
            yield return new RaceSolution(new[]
            {((ICarCommand)new MoveCommand(firstCarRes),
                (ICarCommand)new MoveCommand(secondCarRes))});
        }

        private V ChooseMoveForCar(bool ifFirstCar, RaceState problem, V thisFlag)
        {
            Random rnd = new Random();
            var resList = new List<(List<V>, double)>();
            for (int i = 0; i < 50; i++)
            {
                RaceState state = problem.MakeCopy();
                int allCount = 0;
                var evList = new List<double>();
                List<V> myCommands = new List<V>();
                while (allCount < Depth)
                {
                    var pairInd = rnd.Next(9);
                    var count = rnd.Next(3, 10);
                    var direction = Directions[pairInd];
                    myCommands.Add(direction);
                    allCount += count;
                    for (int j = 0; j < count; j++)
                        GreedyRacer.EvaluateMove(state, evList, direction, ifFirstCar, thisFlag);
                }
                resList.Add((myCommands, evList.Max()));
            }
            var res_V = resList.OrderByDescending(pair => pair.Item2).First().Item1;
            return res_V.First();
        }
    }
}
