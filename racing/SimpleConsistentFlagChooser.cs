using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    internal class SimpleConsistentFlagChooser : IFlagChooser
    {
        public (V FirstCarNextFlag, V SecondCarNextFlag) GetNextFlagsFor(RaceState state)
        {
            var firstCar = state.FirstCar;
            var secondCar = state.SecondCar;

            var flag1 = state.GetNextFlag();
            var flag2 = state.GetNextFlag(1);

            var dist1to1 = flag1.DistTo(firstCar.Pos + firstCar.V);
            var dist2to1 = flag1.DistTo(secondCar.Pos + secondCar.V);

            //правило: если одна из машинок ближе к обоим флагам, то она назначается на первый

            if (dist1to1 <= dist2to1)
                return (flag1, flag2);

            return (flag2, flag1);
        }
    }
}