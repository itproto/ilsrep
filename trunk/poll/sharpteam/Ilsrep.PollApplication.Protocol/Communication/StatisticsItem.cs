using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ilsrep.PollApplication.Communication
{
    public class StatisticsItem : IComparable<StatisticsItem>
    {
        public String userName;
        public Double scores;
        public int attemptsCount;

        #region IComparable<StatisticsItem> Members

        Double IComparable<StatisticsItem>.CompareTo(StatisticsItem otherItem)
        {
            return otherItem.scores.CompareTo(this.scores);
        }

        #endregion
    }
}
