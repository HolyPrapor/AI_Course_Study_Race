using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class MoveCommand : ICarCommand
    {
        public V Acceleration { get; }

        public MoveCommand(V acceleration)
        {
            Acceleration = acceleration;
        }

        public override string ToString()
        {
            return Acceleration.ToString();
        }
    }
}