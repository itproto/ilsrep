using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    /// <summary>
    /// Object that holds choice
    /// </summary>
    [XmlRoot("choice"), Serializable]
    public class Choice
    {
        /// <summary>
        /// Choice id
        /// </summary>
        [XmlAttribute("id")] public int id;
        /// <summary>
        /// Choice
        /// </summary>
        [XmlAttribute("name")] public string choice;
        /// <summary>
        /// Poll that choice belongs to
        /// </summary>
        [XmlIgnore] public Poll parent;
    }
}
