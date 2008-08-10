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
        [XmlIgnore] public const string GET_POLLSESSION_LIST = "getList";
        [XmlIgnore] public const string GET_POLLSESSION = "getPollsession";
        [XmlIgnore] public const string CREATE_POLLSESSION = "createPollsession";
        [XmlIgnore] public const string REMOVE_POLLSESSION = "removePollsession";
        [XmlIgnore] public const string SAVE_RESULT = "saveResult";
        [XmlElement("request")] public Request request = null;
        [XmlElement("pollsessionlist")] public PollSessionList pollSessionList = null;
        [XmlElement("pollsession")] public PollSession pollSession = null;
        [XmlElement("resultslist")] public ResultsList resultsList = null;
    }
}