using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    /// <summary>
    /// Holds a list of poll session ids and names
    /// </summary>
    [XmlRoot("pollsessionlist"), Serializable]
    public class PollSessionList
    {
        /// <summary>
        /// a list of items that holds ids and names
        /// </summary>
        [XmlElement("item", typeof(Item))] public List<Item> items = new List<Item>();
    }
}
