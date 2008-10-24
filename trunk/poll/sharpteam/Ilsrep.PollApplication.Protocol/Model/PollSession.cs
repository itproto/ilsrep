using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using Ilsrep.Common;

namespace Ilsrep.PollApplication.Model
{
    /// <summary>
    /// Object that holds poll session
    /// </summary>
    [XmlRoot("pollsession"), Serializable]
    [DisplayName("PollSession")]
    public class PollSession
    {
        /// <summary>
        /// poll session id
        /// </summary>
        private int _id;
        /// <summary>
        /// poll session name
        /// </summary>
        private string _name;
        /// <summary>
        /// if this poll session is a test
        /// </summary>
        private bool _testMode;
        /// <summary>
        /// if in test mode how much is needed to pass the test
        /// </summary>
        private double _minScore;
        /// <summary>
        /// list of polls that this poll session holds
        /// </summary>
        private List<Poll> _polls = new List<Poll>();
        /// <summary>
        /// Is object serialized
        /// </summary>
        public static bool isSerialized = false;

        [ReadOnly(true)]
        [XmlAttribute("id")]
        public int Id
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

        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value == String.Empty)
                {
                    throw new Exception("PollSession name can't be empty");
                }
                else
                {
                    _name = value;
                }
            }
        }

        [TypeConverter(typeof(bool))]
        [XmlAttribute("testMode")]
        public bool TestMode
        {
            get
            {
                return _testMode;
            }
            set
            {
                _testMode = value;
            }
        }

        [TypeConverter(typeof(double))]
        [XmlAttribute("minScore")]
        public double MinScore
        {
            get
            {
                return _minScore;
            }
            set
            {
                if (value >= 0 && value <= 1)
                {
                    _minScore = value;
                }
                else
                {
                    throw new Exception("minScore must be in interval [0;1]");
                }
            }
        }

        [XmlElement("poll", typeof(Poll))]
        public List<Poll> Polls
        {
            get
            {
                return _polls;
            }
            set
            {
                int index = 0;
                foreach (Poll curPoll in value)
                {
                    if (curPoll.Choices.Count == 0)
                    {
                        throw new Exception("Poll #" + index + " contains 0 choices");
                    }
                    index++;
                }

                _polls = value;
            }
        }
    }
}
