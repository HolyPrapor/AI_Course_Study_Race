using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class MixedMoveChooser : IMoveChooser
    {
        private readonly List<IMoveChooser> Choosers = new List<IMoveChooser>();
        public MixedMoveChooser(int depth = 20,
            double flagsTakenC = 10000, double distC = 1, double nextFlagC = 1 / 4)
        {
            Choosers.Add(new GreedyMoveChooser(new SumWeighter(), depth, flagsTakenC, distC, nextFlagC));
            Choosers.Add(new RandomMoveChooser(new SumWeighter(), depth, flagsTakenC, distC, nextFlagC));
        }
        
        public (ICarCommand FirstCarCommand, ICarCommand SecondCarCommand,
            double Score)[] GetCarCommands(V nextFlagForFirstCar,
            V nextFlagForSecondCar, RaceState raceState, out string debugInfo)
        {
            var results = new List<(ICarCommand FirstCarCommand, ICarCommand SecondCarCommand, double Score)>();
            debugInfo = "";
            foreach (var chooser in Choosers){
                results.AddRange(chooser.GetCarCommands(nextFlagForFirstCar, 
                    nextFlagForSecondCar, raceState, out var debug));
                debugInfo += debug + "\n";
            }
            var best = results.OrderByDescending(tup => tup.Score).First();
            return new[] { best };
        }
    }
}