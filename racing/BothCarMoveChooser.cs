using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    class BothCarMoveChooser : IMoveChooser
    {
        private static readonly List<(V, V)> Directions;
        private readonly int depth;
        private readonly EvaluationFunctions evaluationFunctions;

        static BothCarMoveChooser()
        {
            Directions = new List<(V, V)>();
            // работает и ладно)))
            for(var i = -1; i <= 1; i++)
            {
                for(var j = -1; j <= 1; j++)
                {
                    for(var k = -1; k <= 1; k++)
                    {
                        for(var l = -1; l <= 1; l++)
                        {
                            Directions.Add((new V(i, j), new V(k, l)));
                        }
                    }
                }
            }
        }

        public BothCarMoveChooser(int depth = 20,
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 1d / 4)
        {
            this.depth = depth;
            evaluationFunctions = new EvaluationFunctions(flagsTakenC, distC, nextFlagC);
        }

        public (ICarCommand FirstCarCommand, ICarCommand SecondCarCommand, double Score)[] GetCarCommands
            (V nextFlagForFirstCar, V nextFlagForSecondCar, RaceState raceState, out string debugInfo)
        {
            var (score, firstCarMove, secondCarMove) = ChooseMoveForCars(raceState, nextFlagForFirstCar, nextFlagForSecondCar);
            var carsExchangeScore = double.NegativeInfinity;
            if (raceState.ExchangeCooldown <= 0)
            {
                carsExchangeScore = evaluationFunctions.EvaluateCarsExchange(raceState, nextFlagForFirstCar, nextFlagForSecondCar);
            }
            debugInfo = score.ToString() + "    " + carsExchangeScore.ToString();
            if (carsExchangeScore > score)
            {
                return new[]
                {
                    ((ICarCommand) new ExchangeCommand(),
                        (ICarCommand) new ExchangeCommand(),
                        carsExchangeScore)
                };
            }

            return new[]
            {
                ((ICarCommand) new MoveCommand(firstCarMove),
                    (ICarCommand) new MoveCommand(secondCarMove),
                    score)
            };
        }

        private (double Score, V MoveCar1, V MoveCar2) ChooseMoveForCars(RaceState problem, V flag1, V flag2)
        {
            var dict = Directions.ToDictionary(pair => pair, pair => new List<double>());
            foreach (var pair in dict)
            {
                var state = problem.MakeCopy();
                evaluationFunctions.EvaluateMoves(state, pair.Value, pair.Key, flag1, flag2);
                for (var i = 0; i < depth; i++)
                    evaluationFunctions
                        .EvaluateMoves(state, pair.Value, (V.Zero, V.Zero), flag1, flag2);
            }

            var res_V = dict.OrderByDescending(pair => pair.Value.Max()).First();
            return (res_V.Value.Max(), res_V.Key.Item1, res_V.Key.Item2);
        }
    }
}
