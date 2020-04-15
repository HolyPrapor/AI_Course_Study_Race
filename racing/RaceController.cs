using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AiAlgorithms.Algorithms;
using NUnit.Framework;

namespace AiAlgorithms.racing
{
    public class RaceController
    {
        private const int timeoutPerTick = 100;

        public static RaceState Play(RaceState state, ISolver<RaceState, RaceSolution> racer, bool makeLog)
        {
            if (!makeLog)
                return new RaceController().Play(state, racer, null);
            var filename = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "racing",
                "visualizer",
                "last-log.js"
            );
            using var streamWriter = new StreamWriter(filename);
            var result = new RaceController().Play(state, racer, new JsonGameLogger(streamWriter));
            Console.WriteLine(result.Time + "\t" + result.FirstCar + "\t" + result.SecondCar);
            return result;
        }

        // public IEnumerable<RaceState> Play(RaceState initialState, RaceSolution solution)
        // {
        //     var race = initialState.MakeCopy();
        //     var i = 0;
        //     while (!race.IsFinished && i < solution.Accelerations.Length)
        //     {
        //         var command = solution.Accelerations[i++];
        //         race.FirstCar.NextCommand = command;
        //         race.Tick();
        //         yield return race.MakeCopy();
        //     }
        // }

        public RaceState Play(RaceState initialState, ISolver<RaceState, RaceSolution> solver,
            IGameLogger<RaceTrack, RaceState> logger, int aiTimeoutPerTickMs = timeoutPerTick)
        {
            var race = initialState.MakeCopy();
            logger?.LogStart(race.Track);
            while (!race.IsFinished)
            {
                var variants = solver.GetSolutions(race.MakeCopy(),
                    aiTimeoutPerTickMs).ToList();
                var aiLogger = logger?.GetAiLogger(0);
                LogAiVariants(race, aiLogger, variants);
                var variant = variants.Last();
                var (firstCommand, secondCommand) = variant.CarCommands[0];
                race.FirstCar.NextCommand = firstCommand;
                race.SecondCar.NextCommand = secondCommand;
                logger?.LogTick(race);
                race.Tick();
            }

            logger?.LogEnd(race);
            return race;
        }

        private void LogAiVariants(RaceState state, IGameAiLogger aiLogger, List<RaceSolution> variants)
        {
            var variantsToLog = variants.Cast<RaceSolution>().Reverse().Take(5).ToList();
            //var log = variantsToLog
            //    .Select(v => $"{v.Score.ToString(CultureInfo.InvariantCulture)} {v.CarCommands.StrJoin(",")}")
            //    .StrJoin("\n");
            //aiLogger?.LogText(log);
            aiLogger?.LogText(
                $"{{{string.Join(',', variantsToLog.Select(x => $"Score: {x.Score} DebugInfo: {x.Debug}"))}}}");
            var intensity = 1.0;
            foreach (var solution in variantsToLog)
            {
                var state2 = state.MakeCopy();
                foreach (var a in solution.CarCommands)
                {
                    var startFirst = state2.FirstCar.Pos;
                    var startSecond = state2.SecondCar.Pos;
                    state2.FirstCar.NextCommand = a.firstCarCommand;
                    state2.SecondCar.NextCommand = a.secondCarCommand;
                    state2.Tick();
                    var endFirst = state2.FirstCar.Pos;
                    var endSecond = state2.SecondCar.Pos;
                    aiLogger?.LogLine(startFirst, a.firstCarCommand is ExchangeCommand ? startSecond : endFirst,
                        intensity);
                    aiLogger?.LogLine(startSecond, a.secondCarCommand is ExchangeCommand ? startFirst : endSecond,
                        intensity);
                }

                intensity *= 0.7;
            }
        }
    }
}