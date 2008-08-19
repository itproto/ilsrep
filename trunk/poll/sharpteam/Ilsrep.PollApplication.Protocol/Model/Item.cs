using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    /// <summary>
    /// Item is a generic class to send some data about some object
    /// </summary>
    [XmlRoot("item"), Serializable]
    public class Item
    {
        /// <summary>
        /// ID of an item
        /// </summary>
        [XmlAttribute("id")] public string id;
        /// <summary>
        /// Name of an item
        /// </summary>
        [XmlAttribute("name")] public string name;
    }
}
