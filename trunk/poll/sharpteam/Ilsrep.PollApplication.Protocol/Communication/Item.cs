using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Communication
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
        private int _id;
        /// <summary>
        /// Name of an item
        /// </summary>
        private string _name;

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
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
    }
}
