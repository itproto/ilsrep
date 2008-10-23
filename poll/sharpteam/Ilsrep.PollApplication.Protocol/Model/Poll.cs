using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Helpers;
using System.ComponentModel;
using System.Diagnostics;
using Ilsrep.Common;

namespace Ilsrep.PollApplication.Model
{
    /// <summary>
    /// Object that holds poll
    /// </summary>
    [XmlRoot("poll"), Serializable]
    [TypeConverter(typeof(CollectionTypeConverter))]
    [DisplayName("Poll")]
    public class Poll
    {
        /// <summary>
        /// poll id
        /// </summary>
        private int _id;
        /// <summary>
        /// poll name
        /// </summary>
        private string _name;
        /// <summary>
        /// poll description
        /// </summary>
        private string _description;
        /// <summary>
        /// if poll session in test mode, id of the correct choice
        /// </summary>
        private int _correctChoiceID;
        /// <summary>
        /// if custom choice is enabled for this poll
        /// </summary>
        private bool _customChoiceEnabled;
        /// <summary>
        /// list of choices for this poll
        /// </summary>
        private List<Choice> _choices = new List<Choice>();
        /// <summary>
        /// Generate new negative Poll id
        /// </summary>
        public static IDGenerator pollIDGenerator = new IDGenerator();

        public Poll()
        {
            int newPollID = pollIDGenerator.id;
            Name = "newPoll" + Math.Abs(newPollID);
            Description = "<description>";
            Id = newPollID;
        }

        public Poll(string name)
        {
            Name = name;
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
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value == String.Empty)
                {
                    throw new Exception("Poll name can't be empty");
                }
                else
                {
                    _name = value;
                }
            }
        }

        [XmlElement("description")]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value == String.Empty)
                {
                    throw new Exception("Poll description can't be empty");
                }
                else
                {
                    _description = value;
                }
            }
        }

        [XmlAttribute("correctChoiceID")]
        [DisplayName("CorrectChoiceIndex")]
        [Description("Sequence number of correct choice. Choices indexing begins from 0-index")]
        public int CorrectChoiceID
        {
            get
            {
                return _correctChoiceID;
            }
            set
            {
                _correctChoiceID = value;
            }
        }
        
        [TypeConverter(typeof(bool))]
        [XmlAttribute("customChoiceEnabled")]
        public bool CustomChoiceEnabled
        {
            get
            {
                return _customChoiceEnabled;
            }
            set
            {
                _customChoiceEnabled = value;
            }
        }

        [XmlArray("choices"), XmlArrayItem("choice")]
        public List<Choice> Choices
        {
            get
            {
                return _choices;
            }
            set
            {
                _choices = value;
            }
        }
        
        public int GetChoiceId()
        {
            int index = 0;
            
            Console.WriteLine( "Poll Choices:" );
            foreach ( Choice curChoice in Choices )
            {
                index++;
                Console.WriteLine( index + ". " + curChoice.choice );
            }
            int choiceIndex = InputHelper.AskQuestion( String.Format( "Choose choice [1-{0}]:", index ), 1, index );
            
            return choiceIndex;
        }

        /// <summary>
        /// CollectionTypeConverter
        /// </summary>
        public class CollectionTypeConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (!(value is Poll))
                {
                    Debug.Fail("Value must have Poll type");
                }
                return base.ConvertTo(context, culture, (value as Poll).Name, typeof(String));
            }
        }
    }
}
