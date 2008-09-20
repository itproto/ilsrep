using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Helpers;
using System.ComponentModel;

namespace Ilsrep.PollApplication.Model
{
    /// <summary>
    /// Object that holds poll
    /// </summary>
    [XmlRoot("poll"), Serializable]
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

        [XmlElement("description")]
        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        [XmlAttribute("correctChoiceID")]
        [DisplayName("correctChoiceIndex")]
        [Description("Sequence number of correct choice. Choices indexing begins from 0-index")]
        public int correctChoiceID
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
        public bool customChoiceEnabled
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
        public List<Choice> choices
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
            foreach ( Choice curChoice in choices )
            {
                index++;
                Console.WriteLine( index + ". " + curChoice.choice );
            }
            int choiceIndex = InputHelper.AskQuestion( String.Format( "Choose choice [1-{0}]:", index ), 1, index );
            
            return choiceIndex;
        }
    }
}
