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
using System.Web.Script.Services;

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

        switch (Request["action"])
        {
            case "save":
                int id = Convert.ToInt32(Request["id"]);
                
                try
                {
                    Survey curSurvey = (Session["survey_" + id] as Survey);
                    curSurvey.Name = Request["survey_name"];
                    curSurvey.TestMode = Convert.ToBoolean(Request["survey_test"]);
                    curSurvey.MinScore =  Math.Round( Convert.ToDouble(Request["survey_minscore"])/100, 2 );

                    if (curSurvey.Polls.Count == 0)
                        throw new Exception("Survey must have at least 1 poll");

                    foreach (Poll curPoll in curSurvey.Polls)
                    {
                        if (curPoll.Choices.Count == 0)
                            throw new Exception("Poll `" + curPoll.Name + "` must have at least 1 choice");

                        if (curPoll.CorrectChoiceID == 0)
                            throw new Exception("Poll `" + curPoll.Name + "` must have correct choice set");
                    }

                    if (id <= 0)
                    {
                        PollDAL.CreateSurvey(curSurvey);
                    }
                    else
                    {
                        PollDAL.EditSurvey(curSurvey);
                    }
                    
                    Session["survey_" + id] = null;
                }
                catch (Exception exception)
                {
                    Message = exception.Message;

                    selectedSurvey = Session["survey_" + id] as Survey;
                }
                break;
            case "edit":
                id = Convert.ToInt32(Request["id"]);

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

        surveysList = PollDAL.GetSurveys();
        surveyListMenu.DataSource = surveysList;
        surveyListMenu.DataBind();
    }

    [WebMethod]
    public static IEnumerable<Poll> GetPolls(int surveyID)
    {
        return (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls;
    }

    [WebMethod]
    public static bool AddPoll(Dictionary<string, object> arguments)
    {
        Poll poll = new Poll();

        // trying... if fields are set
        try
        {
            poll.Name = arguments["poll_name"].ToString();
            poll.Description = arguments["poll_desc"].ToString();
            poll.CustomChoiceEnabled = Convert.ToBoolean(arguments["poll_custom"]);
        }
        catch (Exception)
        {
            return false;
        }

        int surveyID = Convert.ToInt32(arguments["surveyID"]);
        (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls.Add(poll);

        return true;
    }

    [WebMethod]
    public static bool EditPoll(Dictionary<string, object> arguments)
    {
        Poll poll = new Poll();

        try
        {
            poll.Id = Convert.ToInt32(arguments["poll_id"]);
            poll.Name = arguments["poll_name"].ToString();
            poll.Description = arguments["poll_desc"].ToString();
            poll.CustomChoiceEnabled = Convert.ToBoolean(arguments["poll_custom"]);
        }
        catch (Exception)
        {
            return false;
        }

        int surveyID = Convert.ToInt32(arguments["surveyID"]);

        // change the poll
        (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls.ForEach(delegate(Poll curPoll)
        {
            if (curPoll.Id == poll.Id)
            {
                curPoll.Name = poll.Name;
                curPoll.Description = poll.Description;
                curPoll.CustomChoiceEnabled = poll.CustomChoiceEnabled;
            }
        });

        return true;
    }

    [WebMethod]
    public static bool RemovePoll(int surveyID, int pollID)
    {
        // does survey exist?
        if (HttpContext.Current.Session["survey_" + surveyID] == null)
            return false;

        Poll poll = (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls.Find(delegate(Poll curPoll) { return curPoll.Id == pollID; });

        if (poll == null)
            return false;

        (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls.Remove(poll);

        return true;
    }

    [WebMethod]
    public static bool AddChoice(Dictionary<string, object> arguments)
    {
        Choice choice = new Choice();

        try
        {
            choice.choice = arguments["choice"].ToString();
        }
        catch (Exception)
        {
            return false;
        }

        int surveyID = Convert.ToInt32(arguments["surveyID"]);
        int pollID = Convert.ToInt32(arguments["poll_id"]);

        (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls.Find(delegate(Poll curPoll) { return curPoll.Id == pollID; }).Choices.Add(choice);
        Poll poll = (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls.Find(delegate(Poll curPoll) { return curPoll.Id == pollID; });

        if (poll.Choices.Count == 1 || poll.CorrectChoiceID == 0)
            (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls.Find(delegate(Poll curPoll) { return curPoll.Id == pollID; }).CorrectChoiceID = choice.Id;

        return true;
    }

    [WebMethod]
    public static bool EditChoice(Dictionary<string, object> arguments)
    {
        Choice choice = new Choice();

        try
        {
            choice.choice = arguments["choice"].ToString();
            choice.Id = Convert.ToInt32(arguments["choice_id"]);
        }
        catch (Exception)
        {
            return false;
        }

        int surveyID = Convert.ToInt32(arguments["surveyID"]);
        int pollID = Convert.ToInt32(arguments["poll_id"]);

        (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls
            .Find(delegate(Poll curPoll) { return curPoll.Id == pollID; }).Choices
            .ForEach(delegate(Choice curChoice)
            {
                if (curChoice.Id == choice.Id)
                {
                    curChoice.choice = choice.choice;
                }
            });

        return true;
    }

    [WebMethod]
    public static bool RemoveChoice(int surveyID, int pollID, int choiceID)
    {
        try
        {
            (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls.ForEach(delegate(Poll curPoll)
            {
                if (curPoll.Id == pollID)
                {
                    Choice choice = curPoll.Choices.Find(delegate(Choice curChoice) { return curChoice.Id == choiceID; });

                    curPoll.Choices.Remove(choice);
                }
            });

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    [WebMethod]
    public static bool SetCorrectChoice(int surveyID, int pollID, int choiceID)
    {
        try
        {
            (HttpContext.Current.Session["survey_" + surveyID] as Survey).Polls.ForEach(delegate(Poll curPoll)
            {
                if (curPoll.Id == pollID)
                {
                   Choice choice = curPoll.Choices.Find(delegate(Choice curChoice) { return curChoice.Id == choiceID; });

                   if (choice != null)
                       curPoll.CorrectChoiceID = choice.Id;
                }
            });

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}