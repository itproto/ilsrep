using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Helpers;

namespace Ilsrep.PollApplication.Model
{
    /// <summary>
    /// Object that holds poll
    /// </summary>
    [XmlRoot("poll"), Serializable]
    public class Poll
    {
        /// <summary>
        /// poll id
        /// </summary>
        [XmlAttribute("id")] public int id;
        /// <summary>
        /// poll name
        /// </summary>
        [XmlAttribute("name")] public string name;
        /// <summary>
        /// poll description
        /// </summary>
        [XmlElement("description")] public string description;
        /// <summary>
        /// if poll session in test mode, id of the correct choice
        /// </summary>
        [XmlAttribute("correctChoiceID")] public int correctChoiceID;
        /// <summary>
        /// if custom choice is enabled for this poll
        /// </summary>
        [XmlAttribute("customChoiceEnabled")] public bool customChoiceEnabled;
        /// <summary>
        /// list of choices for this poll
        /// </summary>
        [XmlArray("choices")] [XmlArrayItem("choice")] public List<Choice> choices = new List<Choice>();

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
