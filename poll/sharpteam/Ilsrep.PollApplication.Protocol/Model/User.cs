using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Model
{
    /// <summary>
    /// Object that holds user information
    /// </summary>
    [XmlRoot("user"), Serializable]
    public class User
    {
        /// <summary>
        /// User name
        /// </summary>
        [XmlAttribute("username")] public string username;
        /// <summary>
        /// Password
        /// </summary>
        [XmlAttribute("password")] public string password = String.Empty;
        /// <summary>
        /// Action(used in user authorisation)
        /// </summary>
        [XmlAttribute("action")] public string action;
        [XmlIgnore] public const string AUTH = "auth";
        [XmlIgnore] public const string NEW_USER = "new";
        [XmlIgnore] public const string EXIST = "exist";

        // Those fields are to save Console Client and Editor functionnallities
        [XmlIgnore]
        public const string LOGIN = "login";
        [XmlIgnore]
        public const string ACCEPTED = "accepted";
        [XmlIgnore]
        public const string DENIED = "denied";
    }
}
