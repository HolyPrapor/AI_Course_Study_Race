using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class GreedyRacer : ISolver<RaceState, RaceSolution>
    {
        private static readonly List<V> Directions;
        private readonly int depth;
        private readonly EvaluationFunctions evaluationFunctions;

        static GreedyRacer()
        {
            var list = new[] {0, 1, -1};
            Directions = list
                .SelectMany(t => list, (t1, t2) => new V(t1, t2))
                .ToList();
        }

        public GreedyRacer(int depth = 20, double flagsTakenC = 10000, double distC = 1, double nextFlagC = 1 / 4)
        {
            this.depth = depth;
            evaluationFunctions = new EvaluationFunctions(flagsTakenC, distC, nextFlagC);
        }

        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var chooser = new SimpleConsistentFlagChooser();
            var pairWeighter = new SumWeighter();
            var pairOfFlags = chooser.GetNextFlagsFor(problem);
            //можно выбирать в choosemove, но тада может сходить только 1
            var firstCarMove = ChooseMoveForCar(true, problem, pairOfFlags.FirstCarNextFlag);
            var secondCarMove = ChooseMoveForCar(false, problem, pairOfFlags.SecondCarNextFlag);
            var firstCarExchangeScore = double.NegativeInfinity;
            var secondCarExchangeScore = double.NegativeInfinity;
            if (problem.ExchangeCooldown <= 0) //ващпе енто уже проверяется в тике
            {
                firstCarExchangeScore = evaluationFunctions
                    .EvaluateExchange(problem, true, pairOfFlags.FirstCarNextFlag);
                secondCarExchangeScore = evaluationFunctions
                    .EvaluateExchange(problem, false, pairOfFlags.SecondCarNextFlag);
            }
            if (pairWeighter.WeightPair(firstCarExchangeScore, secondCarExchangeScore) >
                pairWeighter.WeightPair(firstCarMove.Score, secondCarMove.Score))
                yield return new RaceSolution(new[]
                {
                    ((ICarCommand) new ExchangeCommand(),
                        (ICarCommand) new ExchangeCommand())
                });
            else
                yield return new RaceSolution(new[]
                {
                    ((ICarCommand) new MoveCommand(firstCarMove.Move),
                        (ICarCommand) new MoveCommand(secondCarMove.Move))
                });
        }

        private (double Score, V Move) ChooseMoveForCar(bool ifFirstCar, RaceState problem, V thisFlag)
        {
            var dict = Directions.ToDictionary(pair => pair, pair => new List<double>());
            foreach (var pair in dict)
            {
                var state = problem.MakeCopy();
                evaluationFunctions
                    .EvaluateMove(state, pair.Value, pair.Key, ifFirstCar, thisFlag);
                for (var i = 0; i < depth; i++)
                    evaluationFunctions
                        .EvaluateMove(state, pair.Value, V.Zero, ifFirstCar, thisFlag);
            }

            var res_V = dict.OrderByDescending(pair => pair.Value.Max()).First();
            return (res_V.Value.Max(), res_V.Key);
        }
    }
}