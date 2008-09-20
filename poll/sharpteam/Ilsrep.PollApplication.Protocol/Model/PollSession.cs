using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Ilsrep.PollApplication.Model
{
    /// <summary>
    /// Object that holds poll session
    /// </summary>
    [XmlRoot("pollsession"), Serializable]
    public class PollSession
    {
        /// <summary>
        /// poll session id
        /// </summary>
        public int _id;
        /// <summary>
        /// poll session name
        /// </summary>
        public string _name;
        /// <summary>
        /// if this poll session is a test
        /// </summary>
        public bool _testMode;
        /// <summary>
        /// if in test mode how much is needed to pass the test
        /// </summary>
        public double _minScore;

        /// <summary>
        /// list of polls that this poll session holds
        /// </summary>
        public List<Poll> _polls = new List<Poll>();

        [ReadOnly(true)]
        [XmlAttribute("id")]
        public int id
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
        public string name
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

        [TypeConverter(typeof(bool))]
        [XmlAttribute("testMode")]
        public bool testMode
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
        public double minScore
        {
            get
            {
                return _minScore;
            }
            set
            {
                _minScore = value;
            }
        }

        [XmlElement("poll", typeof(Poll))]
        public List<Poll> polls
        {
            get
            {
                return _polls;
            }
            set
            {
                _polls = value;
            }
        }
    }
}
