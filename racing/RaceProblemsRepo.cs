using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class RaceProblemsRepo
    {

        public static V[] Vectors(string s)
        {
            return s.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                .Select(V.Parse).ToArray();
        }

        public static Disk[] Disks(string s)
        {
            return s.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                .Select(Disk.ParseDisk).ToArray();
        }

        public static IEnumerable<RaceState> Generate(int count, int fieldSize, int flagsCount, Random random) =>
            Enumerable.Range(0, count).Select(i => Generate(fieldSize, flagsCount, random));

        public static RaceState Generate2Lines(int fieldSize, int flagsCount, Random random)
        {
            var flags = Enumerable.Range(0, flagsCount/2)
                .Select(i => new V(10+ 10*i, 0))
                .Concat(Enumerable.Range(0, flagsCount / 2).Select(i => new V(10 + 10 * flagsCount / 2 - 10*i, 20)))
                .ToList();
            var raceTrack = new RaceTrack(flags, new List<Disk>(), fieldSize * 5, flagsCount * 10);
            return new RaceState(raceTrack, new Car(V.Zero, V.Zero, 0));
        }
        public static RaceState GenerateLine(int fieldSize, int flagsCount, Random random)
        {
            var flags = Enumerable.Range(0, flagsCount)
                .Select(i => new V(10 + 10 * i, 0))
                .ToList();
            var raceTrack = new RaceTrack(flags, new List<Disk>(), fieldSize * 10, flagsCount * 3);
            return new RaceState(raceTrack, new Car(V.Zero, V.Zero, 0));
        }
        public static RaceState Generate(int fieldSize, int flagsCount, Random random)
        {
            var flags = Enumerable.Range(0, flagsCount)
                .Select(i => new V(random.Next(-fieldSize, fieldSize), random.Next(-fieldSize, fieldSize)))
                .ToList();
            var raceTrack = new RaceTrack(flags, new List<Disk>(), fieldSize * 5 * flagsCount, flagsCount * 3);
            return new RaceState(raceTrack, new Car(V.Zero, V.Zero, 0));
        }

        public static IEnumerable<RaceState> GetTests()
        {
            yield return new RaceState(
                new RaceTrack(Vectors("100,50 0,0"), Disks(""), 300, 10),
                Car.ParseCar("0,0 0,0 5"));
            yield return new RaceState(
                new RaceTrack(Vectors("50,0 100,0 100,50 100,100 50,50 0,0"), Disks(""), 300, 20),
                Car.ParseCar("0,0 0,0 5"));
            yield return new RaceState(
                new RaceTrack(Vectors("100,10 0,-40 30,50"), Disks(""), 300, 10),
                Car.ParseCar("0,0 0,0 8"));
            yield return new RaceState(
                new RaceTrack(Vectors("25,5 0,-40 -25,5"), Disks(""), 300, 10),
                Car.ParseCar("0,0 0,0 2"));
            yield return new RaceState(
                new RaceTrack(Vectors("400,20 0,-640 -400,20"), Disks(""), 900, 10),
                Car.ParseCar("0,0 0,0 16"));
            var random = new Random(3141592);
            yield return GenerateLine(100, 5, random);
            yield return Generate(100, 8, random);
            yield return Generate(100, 5, random);
            yield return new RaceState(
                new RaceTrack(Vectors("50,-40 100,0 0,0"), Disks("50,0,10 50,10,10 50,-10,10 50,-20,10 50,20,10 50,30,10"), 200, 10),
                Car.ParseCar("0,0 0,0 5"));
            yield return new RaceState(
                new RaceTrack(Vectors("100,0 0,0"), Disks("50,0,10 50,10,10 50,-10,10 50,-20,10 50,20,10 50,30,10"), 200, 10),
                Car.ParseCar("0,0 0,0 5"));
        }
    }
}