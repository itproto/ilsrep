using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Generic;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.DAL;

namespace Ilsrep.PollApplication.WebService
{
    /// <summary>
    /// Web Service for Poll Applications
    /// </summary>
    [WebService(Namespace = "Ilsrep.PollApplication.WebService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PollWebService : System.Web.Services.WebService
    {
        public AuthHeader Authentication = new AuthHeader();
        private const string USER_NAME = "AuthUserName";
        private const string PASSWORD = "AuthPassword";

        /// <summary>
        /// Get list of names and IDs of Surveys from database
        /// </summary>
        /// <returns>List of Surveys</returns>
        [WebMethod]
        [SoapHeader("Authentication")]
        public List<Item> GetSurveys()
        {
            if (Authentication.userName == USER_NAME && Authentication.password == PASSWORD)
            {
                return DAL.PollDAL.GetSurveys();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Survey from database by Survey ID
        /// </summary>
        /// <param name="surveyID">Survey ID that indicates which Survey to get</param>
        /// <returns>Survey object</returns>
        [WebMethod]
        [SoapHeader("Authentication")]
        public Survey GetSurvey(int surveyID)
        {
            if (Authentication.userName == USER_NAME && Authentication.password == PASSWORD)
            {
                return DAL.PollDAL.GetSurvey(surveyID);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create new Survey in database
        /// </summary>
        /// <param name="newSurvey">Survey object that is to be created in database</param>
        /// <returns>ID of just created survey</returns>
        [WebMethod]
        [SoapHeader("Authentication")]
        public int CreateSurvey(Survey newSurvey)
        {
            if (Authentication.userName == USER_NAME && Authentication.password == PASSWORD)
            {
                return DAL.PollDAL.CreateSurvey(newSurvey);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Save changed survey to database
        /// </summary>
        /// <param name="survey">Survey object that is to be changed in database</param>
        [WebMethod]
        [SoapHeader("Authentication")]
        public void EditSurvey(Survey survey)
        {
            if (Authentication.userName == USER_NAME && Authentication.password == PASSWORD)
            {
                DAL.PollDAL.EditSurvey(survey);
            }
        }

        /// <summary>
        /// Save survey results in database
        /// </summary>
        /// <param name="resultsList">List of results</param>
        [WebMethod]
        [SoapHeader("Authentication")]
        public void SaveSurveyResult(SurveyResults resultsList)
        {
            if (Authentication.userName == USER_NAME && Authentication.password == PASSWORD)
            {
                DAL.PollDAL.SaveSurveyResult(resultsList);
            }
        }

        /// <summary>
        /// Select from DB all results of needed Survey
        /// </summary>
        /// <param name="surveyId">Survey ID which results we need</param>
        /// <returns>List of results of needed Survey</returns>
        [WebMethod]
        [SoapHeader("Authentication")]
        public SurveyResults GetSurveyResults(int surveyId)
        {
            if (Authentication.userName == USER_NAME && Authentication.password == PASSWORD)
            {
                return DAL.PollDAL.GetSurveyResults(surveyId);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Remove Survey from database
        /// </summary>
        /// <param name="surveyID">Survey ID that is to be removed</param>
        [WebMethod]
        [SoapHeader("Authentication")]
        public void RemoveSurvey(int surveyID)
        {
            if (Authentication.userName == USER_NAME && Authentication.password == PASSWORD)
            {
                DAL.PollDAL.RemoveSurvey(surveyID);
            }
        }

        /// <summary>
        /// Try to insert new user in DB. The field "user.action" indicates us the 
        /// result of this method. If user.action == user.AUTH then user is successfully
        /// inserted in DB. Otherwise (user.action == user.EXIST), such user already 
        /// exists in DB.
        /// </summary>
        /// <param name="user">User object that will be inserted in DB</param>
        /// <returns>User object</returns>
        [WebMethod]
        [SoapHeader("Authentication")]
        public User RegisterUser(User user)
        {
            if (Authentication.userName == USER_NAME && Authentication.password == PASSWORD)
            {
                return DAL.PollDAL.RegisterUser(user);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Authentication")]
        public User ExistUser(User user)
        {
            if (Authentication.userName == USER_NAME && Authentication.password == PASSWORD)
            {
                return DAL.PollDAL.ExistUser(user);
            }
            else
            {
                return null;
            }
        }

        [WebMethod]
        [SoapHeader("Authentication")]
        public User AuthorizeUser(User user)
        {
            if (Authentication.userName == USER_NAME && Authentication.password == PASSWORD)
            {
                return DAL.PollDAL.AuthorizeUser(user);
            }
            else
            {
                return null;
            }
        }

        public class AuthHeader : SoapHeader
        {
            public string userName;
            public string password;
        }
    }
}
