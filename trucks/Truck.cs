using System.Collections.Generic;

namespace AiAlgorithms.Trucks
{
    public class Truck
    {
        public override string ToString()
        {
            return $"Index: {Index}, UsedVolume: {UsedVolume}, UsedWeight: {UsedWeight}";
        }

        public readonly int Index;

        public readonly double MaxVolume;

        public List<Box> Boxes = new List<Box>();

        public double UsedVolume;

        public double UsedWeight;

        public Truck(in int index, in double maxVolume)
        {
            Index = index;
            MaxVolume = maxVolume;
        }


        public void Put(Box box)
        {
            UsedWeight += box.Weight;
            UsedVolume += box.Volume;
            Boxes.Add(box);
        }

        public void Remove(Box box)
        {
            Boxes.Remove(box);
            UsedWeight -= box.Weight;
            UsedVolume -= box.Volume;
        }
    }
}