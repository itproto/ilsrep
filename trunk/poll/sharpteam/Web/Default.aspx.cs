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

    protected void Page_Load( object sender, EventArgs e )
    {
        PollDAL.connectionString = "Data Source=\"" + Server.MapPath( ConfigurationSettings.AppSettings["dataSource"].ToString() ) + "\"";
        pollSessionsList = PollDAL.GetPollSessions();
        
        if (Request["action"] == "showpollsession")
        {
            pollSession = new PollSession();
            if (Request["do"] == "submitpoll")
            {
                pollSession = (PollSession)Session["pollSession"];

                if (Convert.ToInt32(Session["pollIndex"]) == pollSession.polls.Count - 1)
                {
                    Response.Redirect("?action=results");
                }
                else
                {
                    Session["pollIndex"] = Convert.ToInt32(Session["pollIndex"]) + 1;
                }

                /*
                if (Convert.ToInt32(Request["currentPoll"]) != Convert.ToInt32(Session["pollIndex"]))
                {
                    // Refreshing
                    //Response.Write( 123 );
                    //Response.Redirect( "?" + Request["currentPoll"] + "=" + Session["pollIndex"] );
                }
                else
                {
                    if (Convert.ToInt32(Session["pollIndex"]) == pollSession.polls.Count - 1)
                        Response.Redirect("?action=results");
                    else
                        Session["pollIndex"] = Convert.ToInt32(Session["pollIndex"]) + 1;
                }
                */
            }
            else
            {
                pollSession = PollDAL.GetPollSession(Convert.ToInt32(Request["id"]));
                Session["pollSession"] = pollSession;
                Session["pollIndex"] = 0;
            }
        }
    }
}
