using AiAlgorithms.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiAlgorithms.racing
{
    class DeepFlagChooser
    {
        private static int Depth = 10;
        IEnumerable<int> predictSecondRange = Enumerable
                    .Range(2, 5);
        public (V FirstCarNextFlag, V SecondCarNextFlag) GetNextFlagsFor1(RaceState state,
            IMoveChooser moveChooser)
        {
            var res = predictSecondRange
                .SelectMany(n => new[] { (1, n), (n, 1) })
                .ToDictionary(p=>p, p=>double.NegativeInfinity);
            var firstFlag = state.GetNextFlag(1);
            var leftFlagsSlice = predictSecondRange
                .Select(n => state.GetNextFlag(n))
                .ToArray();
            foreach (var n in predictSecondRange)
            {
                //if (1,n)
                var firstRes = EvaluateFlags(firstFlag, leftFlagsSlice[n],
                        state, moveChooser);
                //if (n,1)
                var secondRes = EvaluateFlags(leftFlagsSlice[n], firstFlag,
                        state, moveChooser);
            }
            var bestPair = res.MaxBy(p => p.Value);
            return (state.GetNextFlag(bestPair.Key.Item1), state.GetNextFlag(bestPair.Key.Item2));
        }

        public double EvaluateFlags(V firstV, V secondV,
            RaceState state, IMoveChooser moveChooser)
        {
            var scoreList = new List<double>();
            var copyState = state.MakeCopy();
            var commadTriplet = moveChooser
                .GetCarCommands(firstV, secondV, state,
                out var debug)
                .MaxBy(t => t.Score);
            state.FirstCar.NextCommand = commadTriplet
                .FirstCarCommand;
            state.SecondCar.NextCommand = commadTriplet
                .SecondCarCommand;
            state.Tick();
            var score = EvaluateState(state, firstV, secondV);
            scoreList.Add(score);
            for (int i = 0; i < Depth; i++)
            {
                var firstFlag = state.GetNextFlag(1);
                var leftFlagsSlice = predictSecondRange
                    .Select(n => state.GetNextFlag(n))
                    .ToArray();
                var commadTriplet1 = moveChooser
                .GetCarCommands(firstV, secondV, state,
                out var debug1)
                .MaxBy(t => t.Score);
                state.FirstCar.NextCommand = commadTriplet1
                    .FirstCarCommand;
                state.SecondCar.NextCommand = commadTriplet1
                    .SecondCarCommand;
                state.Tick();
                var score1 = EvaluateState(state, firstV, secondV);
                scoreList.Add(score1);
            }
            return scoreList.Max(); ;
        }

        public double EvaluateState(RaceState raceState, V firstCarFlag, V secondCarFlag)
        {
            var distance1 = raceState.FirstCar.Pos.DistTo(firstCarFlag);
            var distance2 = raceState.SecondCar.Pos.DistTo(secondCarFlag);
            var alive1 = raceState.FirstCar.IsAlive ? 1 : 0;
            var alive2 = raceState.SecondCar.IsAlive ? 1 : 0;
            return 3000 * raceState.FlagsTaken
                - distance1 - distance2
                + 500 * alive1
                + 500 * alive2;
        }
    }
}
