using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [XmlRoot("request"), Serializable]
    public class Request
    {
        [XmlIgnore] public const string GET_LIST = "getList";
        [XmlIgnore] public const string GET_POLLSESSION = "getPollsession";
        [XmlIgnore] public const string CREATE_POLLSESSION = "createPollsession";
        [XmlIgnore] public const string SAVE_RESULT = "saveResult";
        [XmlAttribute("type")] public string type;
        [XmlAttribute("id")] public string id;
    }
}
