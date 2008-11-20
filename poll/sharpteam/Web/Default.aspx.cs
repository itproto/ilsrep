using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.PollApplication.DAL;
using Ilsrep.Common;
using System.Data.SQLite;

public partial class _Default : System.Web.UI.Page
{
    public SQLiteConnection sqliteConnection = new SQLiteConnection();
    public List<Item> surveysList = new List<Item>();
    public Survey survey = null;
    public ResultsList resultsList = null;
    public string errorMessage = String.Empty;

    protected void Page_Load( object sender, EventArgs e )
    {
        PollDAL.connectionString = "Data Source=\"" + Server.MapPath( ConfigurationSettings.AppSettings["dataSource"].ToString() ) + "\"";
        surveysList = PollDAL.GetSurveys();

        switch (Request["action"])
        {
            case "showsurvey":
                survey = new Survey();
                resultsList = new ResultsList();
                survey = PollDAL.GetSurvey(Convert.ToInt32(Request["id"]));
                Session["survey"] = survey;
                Session["pollIndex"] = 0;
                resultsList.surveyId = survey.Id;
                Session["resultsList"] = resultsList;
                break;
            case "submitpoll":
                survey = (Survey)Session["survey"];
                resultsList = (ResultsList)Session["resultsList"];
                PollResult curResult = new PollResult();
                curResult.questionId = Convert.ToInt32(Session["pollIndex"]);
                curResult.answerId = Convert.ToInt32(Request["choice"]);
                resultsList.results.Add(curResult);
                if (Convert.ToInt32(Session["pollIndex"]) == survey.Polls.Count - 1)
                {
                    Response.Redirect("Default.aspx?action=showresults");
                }
                else
                {
                    Session["pollIndex"] = Convert.ToInt32(Session["pollIndex"]) + 1;
                }
                break;
            case "showresults":
                survey = (Survey)Session["survey"];
                resultsList = (ResultsList)Session["resultsList"];
                // Save results to DB
                break;
        }
    }

    public void LogOut()
    {
        FormsAuthentication.SignOut();
    }
}
