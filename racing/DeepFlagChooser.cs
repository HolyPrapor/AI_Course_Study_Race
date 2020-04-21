using AiAlgorithms.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiAlgorithms.racing
{
    class DeepFlagChooser:IFlagChooser
    {
        //make AbstractRacer to test
        private static int Depth = 10;
        //why generates 2,3,4?
        /*IEnumerable<int> predictSecondRange = Enumerable
                    .Range(2, 3);*/
        //it is better to make array bigger, but it becomes too slow
        int[] predictSecondRange = new[] { 2 };
        IMoveChooser moveChooser;

        public DeepFlagChooser(IMoveChooser mch)
        {
            moveChooser = mch;
        }

        public (V FirstCarNextFlag, V SecondCarNextFlag) GetNextFlagsFor(RaceState state)
        {
            var res = predictSecondRange
                .SelectMany(n => new[] { (1, n), (n, 1) })
                .ToDictionary(p=>p, p=>double.NegativeInfinity);
            var firstFlag = state.GetNextFlag(0);
            var leftFlagsSlice = predictSecondRange
                .Select(n => state.GetNextFlag(n-1))
                .ToArray();
            foreach (var n in predictSecondRange)
            {
                var firstRes = EvaluateFlags(firstFlag, leftFlagsSlice[n-2],
                        state, moveChooser);
                res[(1, n)] = firstRes;
                var secondRes = EvaluateFlags(leftFlagsSlice[n-2], firstFlag,
                        state, moveChooser);
                res[(n, 1)] = secondRes;
            }
            var bestPair = res.MaxBy(p => p.Value);
            return (state.GetNextFlag(bestPair.Key.Item1-1), state.GetNextFlag(bestPair.Key.Item2-1));
        }

        public double EvaluateFlags(V firstV, V secondV,
            RaceState state, IMoveChooser moveChooser)
        {
            var scoreList = new List<double>();
            var copyState = state.MakeCopy();
            var commadTriplet = moveChooser
                .GetCarCommands(firstV, secondV, copyState,
                out var debug)
                .MaxBy(t => t.Score);
            copyState.FirstCar.NextCommand = commadTriplet
                .FirstCarCommand;
            copyState.SecondCar.NextCommand = commadTriplet
                .SecondCarCommand;
            copyState.Tick();
            var score = EvaluateState(copyState, firstV, secondV);
            scoreList.Add(score);
            for (int i = 0; i < Depth; i++)
            {
                var firstFlag = copyState.GetNextFlag(0);
                var leftFlagsSlice = predictSecondRange
                    .Select(n => copyState.GetNextFlag(n-1))
                    .ToArray();
                foreach (var n in predictSecondRange)
                {
                    var commadTriplet1 = moveChooser
                        .GetCarCommands(firstFlag, leftFlagsSlice[n - 2], copyState,
                        out var debug1)
                        .MaxBy(t => t.Score);
                    copyState.FirstCar.NextCommand = commadTriplet1
                        .FirstCarCommand;
                    copyState.SecondCar.NextCommand = commadTriplet1
                        .SecondCarCommand;
                    copyState.Tick();
                    var score1 = EvaluateState(copyState, firstFlag, leftFlagsSlice[n - 2]);
                    scoreList.Add(score1);
                }
            }
            return scoreList.Max(); ;
        }

        public double EvaluateState(RaceState raceState, V firstCarFlag, V secondCarFlag)
        {
            var distance1 = raceState.FirstCar.Pos.DistTo(firstCarFlag);
            var distance2 = raceState.SecondCar.Pos.DistTo(secondCarFlag);
            var alive1 = raceState.FirstCar.IsAlive ? 1 : 0;
            var alive2 = raceState.SecondCar.IsAlive ? 1 : 0;
            return 1000 * raceState.FlagsTaken
                - distance1 - distance2
                + 500 * alive1
                + 500 * alive2;
        }
    }
}
