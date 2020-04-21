using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class DoubleRandomRacer : ISolver<RaceState, RaceSolution>
    {
        private const int Depth = 17;
        private static readonly List<ICarCommand> CarCommands;
        private static readonly List<MoveCommand> MoveCommands;
        private List<(double score, ICarCommand firstCarCommand, ICarCommand secondCarCommand)> PreviousBest;
        
        static DoubleRandomRacer()
        {
            CarCommands = new List<ICarCommand>();
            MoveCommands = new List<MoveCommand>();
            for(var i = -1; i <= 1; i++)
            for (var j = -1; j <= 1; j++)
            {
                MoveCommands.Add(new MoveCommand(new V(i, j)));
                CarCommands.Add(MoveCommands.Last());
            }
            CarCommands.Add(new ExchangeCommand());
        }
        
        public IEnumerable<RaceSolution> GetSolutions(RaceState problem, Countdown countdown)
        {
            var bestScore = double.MinValue;
            List<(double score, ICarCommand firstCarCommand, ICarCommand secondCarCommand)> bestMoves = null;
            //while (!countdown.IsFinished())
            for(var k = 0; k < 200; k++)
            {
                var currentState = problem.MakeCopy();
                var moves = new List<(double score, ICarCommand firstCarCommand, ICarCommand secondCarCommand)>(Depth);
                for (var i = 0; i < Depth; i++)
                {
                    var firstCommand = CarCommands.GetRandomElement();
                    var secondCommand = CarCommands.GetRandomElement();
                    var score = GetScore(currentState, firstCommand, secondCommand);
                    moves.Add((score, firstCommand, secondCommand));
                    if (Math.Abs(score - double.MinValue) < 1)
                        break;
                }
                var maxScore = moves.Max(x => x.score);
                if (maxScore > bestScore)
                {
                    bestScore = maxScore;
                    bestMoves = moves;
                }
            }
            if(PreviousBest != null && PreviousBest.Count > 0)
                if (PreviousBest[0].score > bestScore)
                {
                    bestScore = PreviousBest[0].score;
                    bestMoves = PreviousBest;
                }
            PreviousBest = bestMoves.Skip(1).ToList();
            yield return new RaceSolution(new []{(bestMoves[0].firstCarCommand, bestMoves[0].secondCarCommand)}) { Score = bestScore};
        }

        public static double GetScore(RaceState raceState, ICarCommand firstCarCommand,
            ICarCommand secondCarCommand)
        {
            raceState.FirstCar.NextCommand = firstCarCommand;
            raceState.SecondCar.NextCommand = secondCarCommand;
            raceState.Tick();
            if(!raceState.FirstCar.IsAlive || !raceState.SecondCar.IsAlive)
                return double.MinValue;
            var firstCarDistance = raceState.FirstCar.Pos.DistTo(raceState.GetNextFlag());
            var secondCarDistance = raceState.SecondCar.Pos.DistTo(raceState.GetNextFlag());
            var firstCarBonus = (raceState.FirstCar.Pos + raceState.FirstCar.V).DistTo(raceState.GetNextFlag(1)) -
                                    raceState.FirstCar.Pos.DistTo(raceState.GetNextFlag(1));
            var secondCarBonus = (raceState.SecondCar.Pos + raceState.SecondCar.V).DistTo(raceState.GetNextFlag(1)) - 
                                     raceState.SecondCar.Pos.DistTo(raceState.GetNextFlag(1));
            var firstCarDistanceToClosestFlag =
                raceState.Track.Flags.Select(x => raceState.FirstCar.Pos.DistTo(x)).Min();
            var secondCarDistanceToClosestFlag =
                raceState.Track.Flags.Select(x => raceState.SecondCar.Pos.DistTo(x)).Min();
            var bonusToCurrentFlag = Math.Min(firstCarDistance, secondCarDistance);
            var bonusToNextFlag = Math.Min(firstCarBonus, secondCarBonus);
            var bonusToClosestDistance = Math.Max(firstCarDistanceToClosestFlag, secondCarDistanceToClosestFlag);
            return raceState.FlagsTaken * 2000 - bonusToCurrentFlag * 3 - bonusToNextFlag * 2 - bonusToClosestDistance;
        } 
    }
    
    public static class ListExtensions
    {
        private static Random Random = new Random();
        
        public static T GetRandomElement<T>(this List<T> elements)
        {
            var randomIndex = Random.Next(elements.Count);
            return elements[randomIndex];
        }
    }
}