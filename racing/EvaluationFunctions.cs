using System.Collections.Generic;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    public class EvaluationFunctions
    {
        private readonly double bonusToNextFlagCoef;
        private readonly double distToFlagCoeff;
        private readonly double flagsTakenCoeff;

        public EvaluationFunctions(double flagsTakenc = 10000, double distc = 1, double nextFlagc = 1 / 4)
        {
            flagsTakenCoeff = flagsTakenc;
            distToFlagCoeff = distc;
            bonusToNextFlagCoef = nextFlagc;
        }

        public double EvaluateExchange(RaceState state,
            bool ifFirstCar, V thisFlag)
        {
            return EvaluateCommand(state, ifFirstCar, thisFlag, new ExchangeCommand());
        }

        public void EvaluateMove(RaceState state,
            List<double> evList, V acceleration, bool ifFirstCar, V thisFlag)
        {
            evList.Add(EvaluateCommand(state, ifFirstCar, thisFlag,
                new MoveCommand(acceleration)));
        }

        public double EvaluateCommand(RaceState state,
            bool ifFirstCar, V thisFlag, ICarCommand command)
        {
            var car = ifFirstCar ? state.FirstCar : state.SecondCar;
            car.NextCommand = command;
            state.Tick();
            if (!car.IsAlive) return double.MinValue;
            //var nextFlag = state.GetNextFlag();
            //var bonusToNextFlag = (car.Pos + car.V).DistTo(nextFlag) - car.Pos.DistTo(nextFlag);
            var evaluation =
                flagsTakenCoeff * car.FlagsTaken
                - distToFlagCoeff * thisFlag.DistTo(car.Pos);
                    //- bonusToNextFlagCoef * bonusToNextFlag;
            return evaluation;
        }

        public double EvaluateCarsExchange(RaceState state, V nextFlagForFirstCar, V nextFlagForSecondCar)
        {
            return EvaluateCommands(state, nextFlagForFirstCar, nextFlagForSecondCar, new ExchangeCommand(), new ExchangeCommand());
        }

        public void EvaluateMoves(RaceState state,
            List<double> evList, (V, V) accelerations, V nextFlagForFirstCar, V nextFlagForSecondCar)
        {
            evList.Add(EvaluateCommands(state, nextFlagForFirstCar, nextFlagForSecondCar,
                new MoveCommand(accelerations.Item1), new MoveCommand(accelerations.Item2)));
        }

        public double EvaluateCommands(RaceState state, V nextFlagForFirstCar, V nextFlagForSecondCar,
            ICarCommand commandForFirstCar, ICarCommand commandForSecondCar)
        {
            var car1 = state.FirstCar;
            var car2 = state.SecondCar;
            car1.NextCommand = commandForFirstCar;
            car2.NextCommand = commandForSecondCar;
            state.Tick();
            if (!car1.IsAlive || !car2.IsAlive) return double.MinValue;
            //var nextFlag = state.GetNextFlag();
            //var bonusToNextFlag = (car.Pos + car.V).DistTo(nextFlag) - car.Pos.DistTo(nextFlag);
            var evaluation =
                flagsTakenCoeff * car1.FlagsTaken
                - distToFlagCoeff * nextFlagForFirstCar.DistTo(car1.Pos) +
                flagsTakenCoeff * car2.FlagsTaken
                - distToFlagCoeff * nextFlagForSecondCar.DistTo(car2.Pos);
            //- bonusToNextFlagCoef * bonusToNextFlag;
            return evaluation;
        }
    }
}