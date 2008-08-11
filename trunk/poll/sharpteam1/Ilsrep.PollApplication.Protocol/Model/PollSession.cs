using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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
        [XmlAttribute("id")] public int id;
        /// <summary>
        /// poll session name
        /// </summary>
        [XmlAttribute("name")] public string name;
        /// <summary>
        /// if this poll session is a test
        /// </summary>
        [XmlAttribute("testMode")] public bool testMode;
        /// <summary>
        /// if in test mode how much is needed to pass the test
        /// </summary>
        [XmlAttribute("minScore")] public double minScore;
        /// <summary>
        /// list of polls that this poll session holds
        /// </summary>
        [XmlElement("poll", typeof(Poll))] public List<Poll> polls = new List<Poll>();
    }
}
