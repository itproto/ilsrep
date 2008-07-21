using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [XmlRoot("pollsessionresult"), Serializable]
    public class PollSessionResult
    {
        [XmlAttribute("username")] public string userName;
        [XmlAttribute("pollsessionid")] public int pollsessionId;
        [XmlAttribute("questionid")] public int questionId;
        [XmlAttribute("answerid")] public int answerId;
        [XmlAttribute("customchoice")] public string customChoice;
    }
}