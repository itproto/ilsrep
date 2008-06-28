using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [Serializable]
    public class Poll
    {
        [XmlAttribute("id")] public int id;
        [XmlAttribute("name")] public string name;
        [XmlAttribute("correctChoice")] public int correctChoiceId;
        [XmlElement("description")] public string description;
        public bool customChoice; /* not needed? */
        
        [XmlElement("choices")] public List<Choice> choice = new List<Choice>();
    }
}
