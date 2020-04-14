using System;
using AiAlgorithms.Algorithms;

namespace AiAlgorithms.racing
{
    class MaxDistFlagChooser : IFlagChooser
    {
        public (V FirstCarNextFlag, V SecondCarNextFlag) GetNextFlagsFor(RaceState state)
        {
            V flag1 = null;
            V flag2 = null;
            var maxDist = 0.0;

            for(var i = 1; i < state.Track.Flags.Count; i++)
            {
                var curF1 = state.Track.Flags[i];
                var curF2 = i == state.Track.Flags.Count - 1 ? state.Track.Flags[0] : state.Track.Flags[i + 1];

                var curDist = curF1.DistTo(curF2);
                if (curDist > maxDist)
                {
                    maxDist = curDist;
                    flag1 = curF1;
                    flag2 = curF2;
                }
            }

            var nextFlag = state.GetNextFlag();

            if (nextFlag == flag2)
                flag2 = state.Track.Flags[0];

            var dist1ToNextFlag = nextFlag.DistTo(state.FirstCar.Pos + state.FirstCar.V);
            var dist2ToNextFlag = nextFlag.DistTo(state.SecondCar.Pos + state.SecondCar.V);

            if (dist1ToNextFlag < dist2ToNextFlag)
                return (nextFlag, flag2);

            return (flag2, nextFlag);
        }
    }
}
