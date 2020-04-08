namespace AiAlgorithms.Trucks
{
    public class Box
    {
        public readonly int Index;
        public readonly double Volume;
        public readonly double Weight;

        public Box(int index, double weight, double volume)
        {
            Index = index;
            Weight = weight;
            Volume = volume;
        }

        public override string ToString()
        {
            return $"{Index} {Weight} {Volume}";
        }
    }
}