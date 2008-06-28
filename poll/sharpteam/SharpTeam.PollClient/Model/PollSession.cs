using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [Serializable]
    class PollSession
    {
        [XmlAttribute("id")] public int id;
        [XmlAttribute("name")] public string name;
        [XmlAttribute("testMode")] public bool testMode;
        [XmlAttribute("double")] public double minScore;
        [XmlElement("cars")] public List<Poll> polls = new List<Poll>();
    }
}
