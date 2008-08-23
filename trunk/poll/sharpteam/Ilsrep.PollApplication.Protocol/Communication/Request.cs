using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Communication
{
    /// <summary>
    /// Request object is used to query server
    /// </summary>
    [XmlRoot("request"), Serializable]
    public class Request
    {
        /// <summary>
        /// Request type that is used to get list of poll session
        /// </summary>
        [XmlIgnore] public const string GET_LIST = "getList";
        /// <summary>
        /// Request type that is used to get a specific poll session by id
        /// </summary>
        [XmlIgnore] public const string GET_POLLSESSION = "getPollsession";
        /// <summary>
        /// Request type that is used to create a poll session
        /// </summary>
        [XmlIgnore] public const string CREATE_POLLSESSION = "createPollsession";
        /// <summary>
        /// Request type that is used to edit a poll session
        /// </summary>
        [XmlIgnore] public const string EDIT_POLLSESSION = "editPollsession";
        /// <summary>
        /// Request type that is used to remove a poll session by id
        /// </summary>
        [XmlIgnore] public const string REMOVE_POLLSESSION = "removePollsession";
        /// <summary>
        /// Request type that is used to save result
        /// </summary>
        [XmlIgnore] public const string SAVE_RESULT = "saveResult";
        /// <summary>
        /// string that specifies, which type of request it is
        /// </summary>
        [XmlAttribute("type")] public string type;
        /// <summary>
        /// string that holds an id, which is needed by some of requests
        /// </summary>
        [XmlAttribute("id")] public string id;
    }
}
