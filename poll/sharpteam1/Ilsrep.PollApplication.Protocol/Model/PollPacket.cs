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
        [XmlElement("request")] public Request request = new Request();
        [XmlElement("pollsessionlist")] public PollSessionList pollSessionList = new PollSessionList();
        [XmlElement("pollsession")] public PollSession pollSession = new PollSession();
        [XmlElement("resultslist")] public ResultsList resultsList = new ResultsList();
    }
}