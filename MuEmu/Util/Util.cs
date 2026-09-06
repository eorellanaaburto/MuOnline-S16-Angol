using System;
using System.Collections.Generic;
using System.Text;

namespace MuEmu.Util
{
    static internal class Util
    {
        static public ushort flatStat(double stat)
        {
            long statInt = (long)stat;
            double statFraction = statInt % short.MaxValue;
            statFraction /= short.MaxValue;
            statFraction *= 1000;
            var counter = (ushort)(stat / short.MaxValue);
            return (ushort)(counter * 1000 + statFraction);
        }
    }
}
