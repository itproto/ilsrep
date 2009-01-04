using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ilsrep.PollApplication.Communication
{
    public class StatisticsItem : IComparable<StatisticsItem>
    {
        public String name;
        public Double scores;
        public int attemptsCount;

        #region IComparable<StatisticsItem> Members

        int IComparable<StatisticsItem>.CompareTo(StatisticsItem other)
        {
            return other.scores.CompareTo(this.scores);
        }

        #endregion
    }
}
