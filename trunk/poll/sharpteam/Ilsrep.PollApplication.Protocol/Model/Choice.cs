using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

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
        public int _id;
        /// <summary>
        /// Choice
        /// </summary>
        public string _choice;
        /// <summary>
        /// Poll that choice belongs to
        /// </summary>
        [XmlIgnore]
        public Poll parent;

        [ReadOnly(true)]
        [XmlAttribute("id")]
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        [XmlAttribute("name")]
        public string choice
        {
            get
            {
                return _choice;
            }
            set
            {
                _choice = value;
            }
        }
    }
}
