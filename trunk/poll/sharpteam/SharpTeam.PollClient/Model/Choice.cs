using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    [Serializable]
    public class Choice
    {
        [XmlAttribute("id")] public int id;
        [XmlAttribute("name")] public string choice;
        [XmlIgnore] public Poll parent;
    }
}
