using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [XmlRoot("pollsessionlist"), Serializable]
    public class PollSessionList
    {
        [XmlElement("item", typeof(Item))] public List<Item> items = new List<Item>();
    }
}
