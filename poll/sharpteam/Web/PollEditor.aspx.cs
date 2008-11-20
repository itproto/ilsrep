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
    public List<Item> surveysList = new List<Item>();
    public Survey selectedSurvey = null;
    public string Message = String.Empty;
    public string ShowPage = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        PollDAL.connectionString = "Data Source=\"" + Server.MapPath(ConfigurationSettings.AppSettings["dataSource"].ToString()) + "\"";
        surveysList = PollDAL.GetSurveys();

        switch(Request["action"])
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
            case "editsurvey":
                int id = Convert.ToInt32(Request["id"]);

                if (id == 0)
                {// from session
                    int lastID = Convert.ToInt32(Session["lastID"]);
                    
                    selectedSurvey = new Survey();
                    selectedSurvey.Id = --lastID;
                    
                    Session["lastID"] = lastID;
                }
                else if ((Session["survey_" + id] as Survey) != null && Convert.ToInt32(Request["reset"]) != 1 )
                {
                    selectedSurvey = Session["survey_" + id] as Survey;
                }
                else if (id > 0)
                {
                    selectedSurvey = PollDAL.GetSurvey(id);
                }
                else
                {
                    selectedSurvey = new Survey();
                    selectedSurvey.Id = id;
                }

                if (Session["survey_" + selectedSurvey.Id] == null)
                    Session["survey_" + selectedSurvey.Id] = selectedSurvey;

                ShowPage = "editsurvey";
                break;
        }
    }

    protected void Add(string what)
    {
        switch (what)
        {
            case "poll":
                string poll_name = Request["poll_name"];
                string poll_desc = Request["poll_desc"];
                int id = Convert.ToInt32(Request["survey_id"]);

                if (poll_name == String.Empty || poll_desc == String.Empty)
                {
                    Response.Write("{ response: -1, error: 'At least one of the fields is empty!' }");
                }
                else
                {
                    Poll newPoll = new Poll();
                    newPoll.Name = poll_name;
                    newPoll.Description = poll_desc;

                    (Session["survey_" + id] as Survey).Polls.Add(newPoll);
                    Response.Write("{ response: 1, id: "+newPoll.Id+", poll_name: '"+poll_name+"' }");
                }
                break;
            case "choice":
                string choice = Request["choice"];
                int survey_id = Convert.ToInt32(Request["survey_id"]);
                int poll_id = Convert.ToInt32(Request["poll_id"]);

                if (choice == String.Empty)
                {
                    Response.Write("{ response: -1, error: 'Choice is empty!' }");
                }
                else
                {
                    Choice newChoice = new Choice();
                    newChoice.choice = choice;

                    foreach (Poll poll in (Session["survey_" + survey_id] as Survey).Polls)
                    {
                        if (poll.Id == poll_id)
                        {
                            poll.Choices.Add(newChoice);
                            break;
                        }
                    }

                    Response.Write("{ response: 1, id: " + newChoice.Id + ", poll_id: " + poll_id + ", choice: '" + choice + "' }");
                }

                break;
        }

        Response.End();
    }

    protected void Edit(string what)
    {
        switch (what)
        {
            case "survey":
                int id = Convert.ToInt32(Request["id"]);
                PollDAL.EditSurvey(Session["survey_" + id] as Survey);
                Session["survey_" + id] = null;
                break;
            case "choice":
                int survey_id = Convert.ToInt32(Request["survey_id"]);
                int poll_id = Convert.ToInt32(Request["poll_id"]);
                int choice_id = Convert.ToInt32(Request["choice_id"]);
                string choice = Request["choice"];

                foreach (Poll poll in (Session["survey_" + survey_id] as Survey).Polls)
                {
                    if (poll.Id == poll_id)
                    {
                        foreach (Choice curChoice in poll.Choices)
                        {
                            if (curChoice.Id == choice_id)
                            {
                                curChoice.choice = choice;
                            }
                        }
                        break;
                    }
                }

                Response.Write("{ response: 1, id: " + choice_id + ", poll_id: " + poll_id + ", choice: '" + choice + "' }");
                Response.End();
                break;
        }
    }

    protected void Delete(string what)
    {
        int survey_id, poll_id, choice_id;

        switch (what)
        {
            case "poll":
                survey_id = Convert.ToInt32(Request.QueryString["survey_id"]);
                poll_id = Convert.ToInt32(Request.QueryString["poll_id"]);

                foreach (Poll poll in (Session["survey_" + survey_id] as Survey).Polls)
                {
                    if (poll.Id == poll_id)
                    {
                        (Session["survey_" + survey_id] as Survey).Polls.Remove(poll);
                        break;
                    }
                }

                break;
            case "choice":
                survey_id = Convert.ToInt32(Request.QueryString["survey_id"]);
                poll_id = Convert.ToInt32(Request.QueryString["poll_id"]);
                choice_id = Convert.ToInt32(Request.QueryString["choice_id"]);

                foreach (Poll poll in (Session["survey_" + survey_id] as Survey).Polls)
                {
                    if (poll.Id == poll_id)
                    {
                        foreach (Choice curChoice in poll.Choices)
                        {
                            if (curChoice.Id == choice_id)
                            {
                                poll.Choices.Remove(curChoice);
                                break;
                            }
                        }
                        break;
                    }
                }

                break;
        }
    }
}
