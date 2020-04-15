using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public interface IMoveChooser
    {
        (ICarCommand FirstCarCommand, ICarCommand SecondCarCommand)[] GetCarCommands(V nextFlagForFirstCar,
            V nextFlagForSecondCar, RaceState raceState, out string debugInfo);
    }
}