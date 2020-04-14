using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class RaceSolution : ISolution
    {
        public readonly (ICarCommand firstCarCommand, ICarCommand secondCarCommand)[] CarCommands;
        public readonly IFlagChooser FlagChooser;

        public RaceSolution((ICarCommand firstCarCommand, ICarCommand secondCarCommand)[] carCommands, IFlagChooser flagChooser)
        {
            CarCommands = carCommands;
            FlagChooser = flagChooser;
        }

        public override string ToString()
        {
            return $"Score: {Score}, Commands (TODO:): {CarCommands.StrJoin(" ")}";
        }

        public double Score { get; set; }
    }
}