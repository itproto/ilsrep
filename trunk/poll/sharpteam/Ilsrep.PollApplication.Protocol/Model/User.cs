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
        /// user name
        /// </summary>
        [XmlAttribute("username")] public string username;
        /// <summary>
        /// password
        /// </summary>
        [XmlAttribute("password")] public string password = String.Empty;
        /// <summary>
        /// Is allowed access
        /// </summary>
        [XmlAttribute("auth")] public bool auth = false;
        [XmlAttribute("exist")] public bool exist = false; //temporary
        /// <summary>
        /// If true - server creates new user
        /// </summary>
        [XmlAttribute("new")] public bool isNew = false;
    }
}
