using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public interface IGameLogger<in TStaticState, in TTickState>
    {
        IGameAiLogger GetAiLogger(int playerIndex);
        void LogStart(TStaticState staticState);
        void LogTick(TTickState tickState);
        void LogEnd(RaceState tickState);
    }

    public interface IGameAiLogger
    {
        void LogLine(V start, V end, double intense);
        void LogText(string s);
    }

    public class JsonGameLogger : IGameLogger<RaceTrack, RaceState>
    {
        private readonly Dictionary<int, JsonGameAiLogger> loggers = new Dictionary<int, JsonGameAiLogger>();
        private readonly StreamWriter writer;

        public JsonGameLogger(StreamWriter writer)
        {
            this.writer = writer;
        }

        public IGameAiLogger GetAiLogger(int playerIndex)
        {
            return loggers.GetOrCreate(playerIndex, p => new JsonGameAiLogger());
        }

        public void LogStart(RaceTrack staticState)
        {
            writer.Write(
                $"let originalLog = [{staticState.RaceDuration},[{staticState.Flags.StrJoin(",")}],[{staticState.Obstacles.StrJoin(",")}],[\n");
        }

        public void LogEnd(RaceState tickState)
        {
            var logger = loggers[0];
            writer.Write(
                $"[{tickState.Time}, {(tickState.IsFinished ? 1 : 0)}, {Car2Json(tickState.FirstCar, logger.DebugOutput, logger.DebugLines)}, {Car2Json(tickState.SecondCar, logger.DebugOutput, logger.DebugLines)}]\n");
            writer.Write("]];");
        }

        public void LogTick(RaceState tickState)
        {
            var logger = loggers[0];
            writer.Write(
                $"[{tickState.Time}, {(tickState.IsFinished ? 1 : 0)}, {Car2Json(tickState.FirstCar, logger.DebugOutput, logger.DebugLines)}, {Car2Json(tickState.SecondCar, logger.DebugOutput, logger.DebugLines)}],\n");
            logger.DebugLines.Clear();
            logger.DebugOutput = "";
        }

        private string Car2Json(Car car, string output, List<Line> lines)
        {
            var carJson = string.Join(
                ",", 
                car.Pos, car.V, car.Radius, 
                car.FlagsTaken, 
                car.IsAlive ? 1 : 0, 
                car.NextCommand, 
                $"\"{EscapeJsonString(output)}\"",
                $"[{lines.StrJoin(",", Line2Json)}]");
            return $"[{carJson}]";
        }

        private string Line2Json(Line line)
        {
            return $"[{string.Join(",", line.Start, line.End, line.Intense.ToString(CultureInfo.InvariantCulture))}]";
        }

        private static string EscapeJsonString(string output)
        {
            if (output == null) return "";
            return output.Replace("\"", "\\\"").Replace("'", "\\'").Replace("\n", "\\n");
        }
    }

    public class JsonGameAiLogger : IGameAiLogger
    {
        public string DebugOutput { get; set; }
        public List<Line> DebugLines { get; } = new List<Line>();

        public void LogLine(V start, V end, double intense)
        {
            DebugLines.Add(new Line(start, end, intense));
        }

        public void LogText(string s)
        {
            DebugOutput += s;
        }
    }

    public class Line
    {
        public double Intense;

        public V Start, End;

        public Line(V start, V end, double intense)
        {
            Start = start;
            End = end;
            Intense = intense;
        }
    }
}