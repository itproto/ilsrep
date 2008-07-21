using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [XmlRoot("pollresult"), Serializable]
    public class PollResult
    {
        [XmlAttribute("questionid")] public int questionId;
        [XmlAttribute("answerid")] public int answerId;
        [XmlAttribute("customchoice")] public string customChoice;
    }
}