using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public interface IMoveChooser
    {
        (ICarCommand FirstCarCommand, ICarCommand SecondCarCommand, double Score)[] GetCarCommands(
            V nextFlagForFirstCar,
            V nextFlagForSecondCar, RaceState raceState, out string debugInfo);
    }
}