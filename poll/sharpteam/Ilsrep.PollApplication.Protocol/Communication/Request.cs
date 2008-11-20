using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ilsrep.PollApplication.Communication
{
    /// <summary>
    /// Request object is used to query server
    /// </summary>
    [XmlRoot("request"), Serializable]
    public class Request
    {
        /// <summary>
        /// Request type that is used to get list of survey
        /// </summary>
        [XmlIgnore] public const string GET_LIST = "getList";
        /// <summary>
        /// Request type that is used to get a specific survey by id
        /// </summary>
        [XmlIgnore] public const string GET_SURVEY = "getSurvey";
        /// <summary>
        /// Request type that is used to create a survey
        /// </summary>
        [XmlIgnore] public const string CREATE_SURVEY = "createSurvey";
        /// <summary>
        /// Request type that is used to edit a survey
        /// </summary>
        [XmlIgnore] public const string EDIT_SURVEY = "editSurvey";
        /// <summary>
        /// Request type that is used to remove a survey by id
        /// </summary>
        [XmlIgnore] public const string REMOVE_SURVEY = "removeSurvey";
        /// <summary>
        /// Request type that is used to save result
        /// </summary>
        [XmlIgnore] public const string SAVE_RESULT = "saveResult";
        /// <summary>
        /// Request type that is used to save result
        /// </summary>
        [XmlIgnore] public const string GET_RESULTS = "getResults";
        /// <summary>
        /// Request type that sends user info
        /// </summary>
        [XmlIgnore]
        public const string USER = "user";
        /// <summary>
        /// string that specifies, which type of request it is
        /// </summary>
        [XmlAttribute("type")] public string type;
        /// <summary>
        /// string that holds an id, which is needed by some of requests
        /// </summary>
        [XmlAttribute("id")] public string id;
    }
}
