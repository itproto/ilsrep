using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    /// <summary>
    /// Holds results of user answer
    /// </summary>
    [XmlRoot("pollresult"), Serializable]
    public class PollResult
    {
        /// <summary>
        /// Poll id
        /// </summary>
        [XmlAttribute("questionid")] public int questionId;
        /// <summary>
        /// Choice id
        /// </summary>
        [XmlAttribute("answerid")] public int answerId;
        /// <summary>
        /// If user answered using custom choice then this is used instead of answerId
        /// </summary>
        [XmlAttribute("customchoice")] public string customChoice;
    }
}