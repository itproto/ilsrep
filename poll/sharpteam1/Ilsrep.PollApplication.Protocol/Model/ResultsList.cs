using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [XmlRoot("resultslist"), Serializable]
    public class ResultsList
    {
        [XmlElement("pollresult", typeof(PollResult))] public List<PollResult> results = new List<PollResult>();
        [XmlAttribute("username")] public string userName;
        [XmlAttribute("pollsessionid")] public int pollsessionId;
    }
}