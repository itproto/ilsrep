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
        [XmlElement("description")] public string description;
        [XmlAttribute("correctChoiceID")] public int correctChoiceID;
        [XmlAttribute("customChoiceEnabled")] public bool customChoiceEnabled;
        
        [XmlArray("choices")] [XmlArrayItem("choice")] public List<Choice> choices = new List<Choice>();
    }
}
