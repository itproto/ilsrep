using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [XmlRoot("poll"), Serializable]
    public class Poll
    {
        [XmlAttribute("id")] public int id;
        [XmlAttribute("name")] public string name;
        [XmlAttribute("correctChoice")] public int correctChoiceId;
        [XmlElement("description")] public string description;
        [XmlIgnore] public bool customChoice; /* not needed? */
        
        [XmlArray("choices")] [XmlArrayItem("choice")] public List<Choice> choices = new List<Choice>();
    }
}
