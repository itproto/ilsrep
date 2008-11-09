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
    public List<Item> pollSessionsList = new List<Item>();
    public PollSession pollSession = null;
    public ResultsList resultsList = null;
    public string errorMessage = String.Empty;

    protected void Page_Load( object sender, EventArgs e )
    {
        PollDAL.connectionString = "Data Source=\"" + Server.MapPath( ConfigurationSettings.AppSettings["dataSource"].ToString() ) + "\"";
        pollSessionsList = PollDAL.GetPollSessions();

        switch (Request["action"])
        {
            case "showpollsession":
                pollSession = new PollSession();
                resultsList = new ResultsList();
                pollSession = PollDAL.GetPollSession(Convert.ToInt32(Request["id"]));
                Session["pollSession"] = pollSession;
                Session["pollIndex"] = 0;
                resultsList.pollsessionId = pollSession.Id;
                Session["resultsList"] = resultsList;
                break;
            case "submitpoll":
                pollSession = (PollSession)Session["pollSession"];
                resultsList = (ResultsList)Session["resultsList"];
                PollResult curResult = new PollResult();
                curResult.questionId = Convert.ToInt32(Session["pollIndex"]);
                curResult.answerId = Convert.ToInt32(Request["choice"]);
                resultsList.results.Add(curResult);
                if (Convert.ToInt32(Session["pollIndex"]) == pollSession.Polls.Count - 1)
                {
                    Response.Redirect("Default.aspx?action=showresults");
                }
                else
                {
                    Session["pollIndex"] = Convert.ToInt32(Session["pollIndex"]) + 1;
                }
                break;
            case "showresults":
                pollSession = (PollSession)Session["pollSession"];
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
