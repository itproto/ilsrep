using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    /// <summary>
    /// Root elements that is transfered between poll client, editor and server
    /// </summary>
    [XmlRoot("pollpacket"), Serializable]
    public class PollPacket
    {
        /// <summary>
        /// Request element is used to query server
        /// </summary>
        [XmlElement("request")] public Request request = null;
        /// <summary>
        /// PollSessionList is sent on GET_LIST request
        /// </summary>
        [XmlElement("pollsessionlist")] public PollSessionList pollSessionList = null;
        /// <summary>
        /// PollSession is sent on GET_POLLSESSION request
        /// </summary>
        [XmlElement("pollsession")] public PollSession pollSession = null;
        /// <summary>
        /// Results list is sent when user finishes poll in poll client
        /// </summary>
        [XmlElement("resultslist")] public ResultsList resultsList = null;
    }
}