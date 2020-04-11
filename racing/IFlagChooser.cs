using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public interface IFlagChooser
    {
        (V FirstCarNextFlag, V SecondCarNextFlag) GetNextFlagsFor(RaceState state);
    }
}