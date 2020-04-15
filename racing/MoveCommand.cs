using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class MoveCommand : ICarCommand
    {
        public MoveCommand(V acceleration)
        {
            Acceleration = acceleration;
        }

        public V Acceleration { get; }

        public override string ToString()
        {
            return Acceleration.ToString();
        }
    }
}