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
        [XmlElement("pollsessionresult", typeof(PollSessionResult))] public List<PollSessionResult> results = new List<PollSessionResult>();
        [XmlAttribute("username")] public string userName;
        [XmlAttribute("pollsessionid")] public int pollsessionId;
    }
}