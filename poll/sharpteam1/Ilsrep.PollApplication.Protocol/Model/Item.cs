using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [XmlRoot("item"), Serializable]
    public class Item
    {
        [XmlAttribute("id")] public string id;
        [XmlAttribute("name")] public string name;
    }
}
