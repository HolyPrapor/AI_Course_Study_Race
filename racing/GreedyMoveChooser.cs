using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class GreedyMoveChooser : IMoveChooser
    {
        private readonly IPairWeighter PairWeighter;
        private static readonly List<V> Directions;
        private readonly int depth;
        private readonly EvaluationFunctions evaluationFunctions;

        static GreedyMoveChooser()
        {
            var list = new[] {0, 1, -1};
            Directions = list
                .SelectMany(t => list, (t1, t2) => new V(t1, t2))
                .ToList();
        }
        
        public GreedyMoveChooser(IPairWeighter weighter, int depth = 20,
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 1 / 4)
        {
            PairWeighter = weighter;
            this.depth = depth;
            evaluationFunctions = new EvaluationFunctions(flagsTakenC, distC, nextFlagC);
        }
        
        public (ICarCommand FirstCarCommand, ICarCommand SecondCarCommand)[] GetCarCommands(V nextFlagForFirstCar,
            V nextFlagForSecondCar, RaceState raceState, out string debugInfo)
        {
            //ìîæíî âûáèðàòü â choosemove, íî òàäà ìîæåò ñõîäèòü òîëüêî 1
            var firstCarMove = ChooseMoveForCar(true, raceState, nextFlagForFirstCar);
            var secondCarMove = ChooseMoveForCar(false, raceState, nextFlagForSecondCar);
            var firstCarExchangeScore = double.NegativeInfinity;
            var secondCarExchangeScore = double.NegativeInfinity;
            if (raceState.ExchangeCooldown <= 0) //âàùïå åíòî óæå ïðîâåðÿåòñÿ â òèêå
            {
                firstCarExchangeScore = evaluationFunctions
                    .EvaluateExchange(raceState, true, nextFlagForFirstCar);
                secondCarExchangeScore = evaluationFunctions
                    .EvaluateExchange(raceState, false, nextFlagForSecondCar);
            }
            debugInfo = "";
            if (PairWeighter.WeightPair(firstCarExchangeScore, secondCarExchangeScore) >
                PairWeighter.WeightPair(firstCarMove.Score, secondCarMove.Score))
                return new[] {((ICarCommand) new ExchangeCommand(), (ICarCommand) new ExchangeCommand())};
            return new[]
            {
                ((ICarCommand) new MoveCommand(firstCarMove.Move),
                    (ICarCommand) new MoveCommand(secondCarMove.Move))
            };
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