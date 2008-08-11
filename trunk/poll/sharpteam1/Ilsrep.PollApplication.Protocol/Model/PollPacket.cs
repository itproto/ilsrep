using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [XmlRoot("pollpacket"), Serializable]
    public class PollPacket
    {
        [XmlElement("request")] public Request request = null;
        [XmlElement("pollsessionlist")] public PollSessionList pollSessionList = null;
        [XmlElement("pollsession")] public PollSession pollSession = null;
        [XmlElement("resultslist")] public ResultsList resultsList = null;
    }
}