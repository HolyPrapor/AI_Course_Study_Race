using System;
using System.Collections.Generic;
using System.Globalization;
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
                return new RaceController().Play(state, racer, null, timeoutPerTick);
            var filename = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "racing",
                "visualizer",
                "last-log.js"
            );
            using var streamWriter = new StreamWriter(filename);
            var result = new RaceController().Play(state, racer, new JsonGameLogger(streamWriter), timeoutPerTick);
            Console.WriteLine(result.Time + "\t" + result.Car);
            return result;
        }

        public IEnumerable<RaceState> Play(RaceState initialState, RaceSolution solution)
        {
            var race = initialState.MakeCopy();
            var i = 0;
            while (!race.IsFinished && i < solution.Accelerations.Length)
            {
                var command = solution.Accelerations[i++];
                race.Car.NextCommand = command;
                race.Tick();
                yield return race.MakeCopy();
            }
        }

        public RaceState Play(RaceState initialState, ISolver<RaceState, RaceSolution> solver,
            IGameLogger<RaceTrack, RaceState> logger, int aiTimeoutPerTickMs = timeoutPerTick)
        {
            var race = initialState.MakeCopy();
            logger?.LogStart(race.Track);
            while (!race.IsFinished)
            {
                var variants = solver.GetSolutions(race, aiTimeoutPerTickMs).ToList();
                var aiLogger = logger?.GetAiLogger(0);
                LogAiVariants(race, aiLogger, variants);
                var command = variants.Last().Accelerations[0];
                race.Car.NextCommand = command;
                logger?.LogTick(race);
                race.Tick();
            }
            logger?.LogEnd(race);
            return race;
        }

        private void LogAiVariants(RaceState state, IGameAiLogger aiLogger, List<RaceSolution> variants)
        {
            var variantsToLog = variants.Cast<RaceSolution>().Reverse().Take(5).ToList();
            var log = variantsToLog.Select(v => $"{v.Score.ToString(CultureInfo.InvariantCulture)} {v.Accelerations.StrJoin(",")}").StrJoin("\n");
            aiLogger?.LogText(log);

            var intensity = 1.0;
            foreach (var solution in variantsToLog)
            {
                var state2 = state.MakeCopy();
                foreach (var a in solution.Accelerations)
                {
                    var start = state2.Car.Pos;
                    state2.Car.NextCommand = a;
                    state2.Tick();
                    var end = state2.Car.Pos;
                    aiLogger?.LogLine(start, end, intensity);
                }
                intensity *= 0.7;
            }
        }
    }
}