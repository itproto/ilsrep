using System;
using System.Configuration;
using System.Collections;
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
using System.Web.Services;

public partial class _PollEditor : System.Web.UI.Page
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
        surveyListMenu.DataSource = surveysList;
        surveyListMenu.DataBind();

        switch(Request["action"])
        {
            case "edit":
                int id = Convert.ToInt32(Request["id"]);

                if (id == 0)
                {
                    if (Convert.ToInt32(Request["reset"]) == 1 || Session["survey_" + id] == null)
                    {
                        selectedSurvey = new Survey();
                        selectedSurvey.Id = id;
                    }
                    else
                    {
                        selectedSurvey = Session["survey_" + id] as Survey;
                    }
                }
                else if (id > 0)
                {
                    if (Convert.ToInt32(Request["reset"]) == 1 || Session["survey_" + id] == null)
                    {
                        selectedSurvey = PollDAL.GetSurvey(id);
                    }
                    else
                    {
                        selectedSurvey = Session["survey_" + id] as Survey; 
                    }
                }

                if (Session["survey_" + selectedSurvey.Id] == null)
                    Session["survey_" + selectedSurvey.Id] = selectedSurvey;

                //listPolls.DataSource = selectedSurvey.Polls;
                break;
        }
    }

    [WebMethod]
    public static IEnumerable<Poll> GetPolls(int surveyID)
    {
        return (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls;
    }

    protected void Add(string what)
    {
        switch (what)
        {
            case "poll":
                string poll_name = Request["poll_name"];
                string poll_desc = Request["poll_desc"];
                bool poll_custom = Convert.ToBoolean(Request["poll_custom"]);
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
                    newPoll.CustomChoiceEnabled = poll_custom;

                    (Session["survey_" + id] as Survey).Polls.Add(newPoll);
                    Response.Write("{ response: 1, id: "+newPoll.Id+", poll_name: '"+poll_name+"', poll_custom: '"+poll_custom.ToString()+"' }");
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
                            if (poll.Choices.Count == 0)
                                poll.CorrectChoiceID = newChoice.Id;
                            
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
                Survey curSurvey = (Session["survey_" + id] as Survey);
                (Session["survey_" + id] as Survey).Name = Request["survey_name"];

                try
                {
                    if (curSurvey.Polls.Count == 0)
                        throw new Exception("Survey must have at least 1 poll");

                    foreach (Poll curPoll in curSurvey.Polls)
                    {
                        if (curPoll.Choices.Count == 0)
                            throw new Exception("Poll `" + curPoll.Name + "` must have at least 1 choice");
                    }

                    if (id < 0)
                    {
                        PollDAL.CreateSurvey(Session["survey_" + id] as Survey);
                    }
                    else
                    {
                        PollDAL.EditSurvey(Session["survey_" + id] as Survey);
                    }
                    Session["survey_" + id] = null;
                }
                catch (Exception exception)
                {
                    //MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Message = exception.Message;
//                    Show("editsurvey");
                }

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
                        if (poll.Choices.Count > 1 && poll.CorrectChoiceID == choice_id)
                        {
                            Response.Write("{ response: -1, error: 'Correct choice can`t be deleted!' }");
                        }
                        else
                        {
                            foreach (Choice curChoice in poll.Choices)
                            {
                                if (curChoice.Id == choice_id)
                                {
                                    poll.Choices.Remove(curChoice);
                                    break;
                                }
                            }

                            Response.Write("{ response: 1, id: "+choice_id+" }");
                        }
                        break;
                    }
                }

                break;
        }

        Response.End();
    }

    protected void SetCorrectChoice(int survey_id, int poll_id, int choice_id)
    {
        //Poll currentPoll = (Session["survey_" + survey_id] as Survey).Polls.Find(delegate(Poll poll) { return poll.Id == poll_id; });
        //Choice currentChoice = poll.Choices.Find(delegate(Choice choice) { return choice.Id == choice_id; });
        //currentPoll.Choices.F






        foreach (Poll poll in (Session["survey_" + survey_id] as Survey).Polls)
        {
            if (poll.Id == poll_id)
            {
                foreach (Choice curChoice in poll.Choices)
                {
                    if (curChoice.Id == choice_id)
                    {
                        poll.CorrectChoiceID = choice_id;
                        return;
                    }
                }
                break;
            }
        }
    }

}
