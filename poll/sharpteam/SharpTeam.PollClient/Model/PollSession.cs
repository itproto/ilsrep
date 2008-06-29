using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [XmlRoot("pollsession"), Serializable]
    public class PollSession
    {
        [XmlAttribute("id")] public int id;
        [XmlAttribute("name")] public string name;
        [XmlAttribute("testMode")] public bool testMode;
        [XmlAttribute("minScore")] public double minScore;
        [XmlArray("polls"), XmlArrayItem("poll", typeof(Poll) )] public List<Poll> polls = new List<Poll>();
    }
}
