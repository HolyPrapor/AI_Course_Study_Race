using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class RaceSolution : ISolution
    {
        public readonly (ICarCommand firstCarCommand, ICarCommand secondCarCommand)[] CarCommands;

        public RaceSolution((ICarCommand firstCarCommand, ICarCommand secondCarCommand)[] carCommands)
        {
            CarCommands = carCommands;
        }

        public override string ToString()
        {
            return $"Score: {Score}, Commands (TODO:): {CarCommands.StrJoin(" ")}";
        }

        public double Score { get; set; }
    }
}