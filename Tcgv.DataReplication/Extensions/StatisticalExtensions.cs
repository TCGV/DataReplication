using System;
using System.Collections.Generic;
using System.Linq;

namespace Tcgv.DataReplication.Extensions
{
    public static class StatisticalExtensions
    {
        public static double StdDev(this IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }
    }
}
