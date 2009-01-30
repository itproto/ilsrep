using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ilsrep.PollApplication.Communication
{
    public class StatisticsItem : IComparable<StatisticsItem>
    {
        private string _id;
        private string _name;
        private Double _scores;
        private int _attemptsCount;

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public String name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public Double scores
        {
            get
            {
                return _scores;
            }
            set
            {
                _scores = value;
            }
        }

        public int attemptsCount
        {
            get
            {
                return _attemptsCount;
            }
            set
            {
                _attemptsCount = value;
            }
        }

        public string GetScores
        {
            get
            {
                return String.Format("{0:G4}%", _scores);
            }
        }

        #region IComparable<StatisticsItem> Members

        int IComparable<StatisticsItem>.CompareTo(StatisticsItem other)
        {
            return other.scores.CompareTo(this.scores);
        }

        #endregion
    }
}
