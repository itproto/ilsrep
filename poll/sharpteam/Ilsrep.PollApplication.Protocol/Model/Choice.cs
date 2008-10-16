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
    [TypeConverter(typeof(CollectionTypeConverter))]
    [DisplayName("Choice")]
    public class Choice
    {
        /// <summary>
        /// Choice id
        /// </summary>
        private int _id;
        /// <summary>
        /// Choice
        /// </summary>
        private string _choice;
        /// <summary>
        /// Poll that choice belongs to
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
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
                if (value == String.Empty)
                {
                    throw new Exception("Choice can't be empty");
                }
                else
                {
                    _choice = value;
                }
            }
        }

        /// <summary>
        /// CollectionTypeConverter
        /// </summary>
        public class CollectionTypeConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                return base.ConvertTo(context, culture, "Choice", typeof(String));
            }
        }
    }
}
