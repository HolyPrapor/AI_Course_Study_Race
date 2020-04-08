using System.Globalization;
using System.Linq;

namespace AiAlgorithms.Trucks
{
    public class TrucksProblem
    {
        public readonly Box[] Boxes;

        public readonly double TargetTruckWeight;
        public readonly int TrucksCount;

        public readonly double TruckVolume;

        public TrucksProblem(double truckVolume, int trucksCount, Box[] boxes)
        {
            TruckVolume = truckVolume;
            TrucksCount = trucksCount;
            Boxes = boxes;
            TargetTruckWeight = Boxes.Sum(b => b.Weight) / TrucksCount;
        }

        public override string ToString()
        {
            return $"{Boxes.Length} boxes {1 - Boxes.Sum(b => b.Volume) / TrucksCount / TruckVolume:0%} free volume";
        }

        public static TrucksProblem LoadFrom(string[] lines)
        {
            var boxes = lines
                .Select(line => line.Split(' ').Select(p => double.Parse(p, CultureInfo.InvariantCulture)).ToArray())
                .Select((wv, i) => new Box(i, wv[0], wv[1]))
                .ToArray();
            return new TrucksProblem(100, 100, boxes);
        }
    }
}