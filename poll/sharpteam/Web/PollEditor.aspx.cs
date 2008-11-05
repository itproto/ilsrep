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

        switch (Request["editoraction"])
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
            case "delete":
                Delete(Request["what"]);
                break;
        }
    }

    protected void Show(string what)
    {
        switch (what)
        {
            case "pollsession":
                int id = Convert.ToInt32(Request["id"]);

                if (id == 0)
                {// from session
                    int lastID = Convert.ToInt32(Session["lastID"]);
                    
                    selectedPollsession = new PollSession();
                    selectedPollsession.Id = --lastID;
                    
                    Session["lastID"] = lastID;
                }
                else if ((Session["pollsession_" + id] as PollSession) != null && Convert.ToInt32(Request["reset"]) != 1 )
                {
                    selectedPollsession = Session["pollsession_" + id] as PollSession;
                }
                else if (id > 0)
                {
                    selectedPollsession = PollDAL.GetPollSession(id);
                }
                else
                {
                    selectedPollsession = new PollSession();
                    selectedPollsession.Id = id;
                }

                if (Session["pollsession_" + selectedPollsession.Id] == null)
                    Session["pollsession_" + selectedPollsession.Id] = selectedPollsession;

                break;
        }
    }

    protected void Add(string what)
    {
        switch (what)
        {
            case "poll":
                string poll_name = Request["poll_name"];
                string poll_description = Request["poll_description"];

                int id = 0;
                try
                {
                    id = Convert.ToInt32(Request["pollsession_id"]);
                }
                catch (Exception)
                {
                    Response.Write("Error!");
                    Response.End();
                    return;
                }

                if (poll_name == String.Empty || poll_description == String.Empty)
                {
                    Response.Write("Error: At least one of the fields is empty!");
                }
                else
                {
                    Poll newPoll = new Poll();
                    newPoll.Name = poll_name;
                    newPoll.Description = poll_description;

                    (Session["pollsession_" + id] as PollSession).Polls.Add(newPoll);
                    Response.Write("");
                }

                Response.End();
                break;

            case "choice":
                string choice = Request["choice_name"];

                int pollsessionId = 0;
                int pollId = 0;
                try
                {
                    pollsessionId = Convert.ToInt32(Request["pollsession_id"]);
                    pollId = Convert.ToInt32(Request["poll_id"]);
                }
                catch (Exception)
                {
                    Response.Write("Error!");
                    Response.End();
                    return;
                }

                if (choice == String.Empty)
                {
                    Response.Write("Error: Field is empty!");
                }
                else
                {
                    Choice newChoice = new Choice();
                    newChoice.choice = choice;

                    //(Session["pollsession_" + pollsessionId] as PollSession).Polls[pollId].Choices.Add(newChoice);
                    Response.Write("");
                }

                Response.End();
                break;
        }
    }

    protected void Edit(string what)
    {
        switch (what)
        {
            case "pollsession":
                int id = Convert.ToInt32(Request["id"]);
                PollDAL.EditPollSession(Session["pollsession_" + id] as PollSession);
                Session["pollsession_" + id] = null;
                break;
        }
    }

    protected void Delete(string what)
    {
        switch (what)
        {
            case "poll":
                int pollsession_id = Convert.ToInt32(Request["pollsession_id"]);
                int poll_id = Convert.ToInt32(Request["poll_id"]);

                foreach (Poll poll in (Session["pollsession_" + pollsession_id] as PollSession).Polls)
                {
                    if (poll.Id == poll_id)
                    {
                        (Session["pollsession_" + pollsession_id] as PollSession).Polls.Remove(poll);
                        break;
                    }
                }

                break;
        }
    }
}
