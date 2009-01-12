using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Ilsrep.PollApplication.Model;

namespace Ilsrep.PollApplication.Communication
{
    /// <summary>
    /// Result list after survey
    /// </summary>
    [XmlRoot("resultslist"), Serializable]
    public class SurveyResults
    {
        /// <summary>
        /// List of the poll results
        /// </summary>
        [XmlElement("pollresult", typeof(PollResult))] public List<PollResult> results = new List<PollResult>();
        /// <summary>
        /// Username of the user, submiting the results
        /// </summary>
        [XmlAttribute("username")] public string userName;
        /// <summary>
        /// Survey id, to which the results are related
        /// </summary>
        [XmlAttribute("pollsessionid")] public int surveyId; // <-- surveyid
    }
}