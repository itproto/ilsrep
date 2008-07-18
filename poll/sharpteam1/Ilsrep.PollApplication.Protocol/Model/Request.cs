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
        [XmlIgnore] public string TYPE_LIST = "list";
        [XmlIgnore] public string TYPE_POLLXML = "pollxml";
        [XmlAttribute("type")] public string type;
        [XmlAttribute("id")] public int id;
    }
}
