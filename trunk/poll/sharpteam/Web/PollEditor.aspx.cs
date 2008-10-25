using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.PollApplication.DAL;
using Ilsrep.Common;
using System.Data.SQLite;

public partial class PollEditor : System.Web.UI.Page
{
    public SQLiteConnection sqliteConnection = new SQLiteConnection();
    public List<Item> pollSessionsList = new List<Item>();
    public PollSession selectedPollsession = null;
    public string Message = String.Empty;
    public string ShowPage = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        PollDAL.connectionString = "Data Source=\"" + Server.MapPath(ConfigurationSettings.AppSettings["dataSource"].ToString()) + "\"";
        pollSessionsList = PollDAL.GetPollSessions();

        switch(Request["do"])
        {
            case "show":
                Show(Request["what"]);
                break;
            case "add":
                Add(Request["what"]);
                break;
            case "edit":
                Edit(Request["what"]);
                break;
        }
    }

    protected void Show(string what)
    {
        switch (what)
        {
            case "addpollsession":
                ShowPage = "addpollsession";

                break;
            case "editpollsession":
                int id = Convert.ToInt32(Request["id"]);

                if (id == -1)
                {// from session
                    selectedPollsession = Session["newPollSession"] as PollSession;
                }
                else
                {
                    selectedPollsession = PollDAL.GetPollSession(id);
                }

                ShowPage = "editpollsession";

                break;
        }
    }

    protected void Add(string what)
    {
        switch (what)
        {
            case "pollsession":
                if (Request["pollsession_name"] == null || Request["pollsession_name"].ToString().Length == 0)
                {
                    Message = "Couldn't add Pollsession: Field `Pollsession Name` is empty!";
                    Show("addpollsession");
                }

                Session["newPollSession"] = new PollSession();
                (Session["newPollSession"] as PollSession).Name = Request["pollsession_name"];

                Response.Redirect("PollEditor.aspx?do=show&what=editpollsession&id=-1");

                break;
        }
    }

    protected void Edit(string what)
    {
        switch (what)
        {
            case "pollsession":
                
                break;
        }
    }
}
