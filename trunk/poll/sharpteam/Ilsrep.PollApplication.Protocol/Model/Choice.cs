using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Diagnostics;
using Ilsrep.Common;

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
        /// <summary>
        /// Generate new negative Choice id
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public static IDGenerator choiceIDGenerator = new IDGenerator();

        public Choice()
        {
            if (!(Survey.isSerialized))
            {
                int newChoiceID = choiceIDGenerator.id;
                choice = "newChoice" + Math.Abs(newChoiceID);
                Id = newChoiceID;
            }
        }

        public Choice(string pChoice)
        {
            choice = pChoice;
        }

        [ReadOnly(true)]
        [XmlAttribute("id")]
        public int Id
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
        [DisplayName("Choice")]
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
                if (!(value is Choice))
                {
                    Debug.Fail("Value must have Choice type");
                }
                return base.ConvertTo(context, culture, (value as Choice).choice, typeof(String));
            }
        }
    }
}
