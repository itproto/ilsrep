using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.DAL;
using Ilsrep.PollApplication.Communication;

public partial class SurveyControl : System.Web.UI.UserControl
{
    protected Survey survey
    {
        get
        {
            if (Session["survey"] == null && SurveyID > 0)
            {
                Session["survey"] = PollDAL.GetSurvey(SurveyID);
            }

            return (Survey)Session["survey"];
        }
        set
        {
            Session["survey"] = value;
        }
    }

    protected int pollIndex
    {
        get
        {
            return Convert.ToInt32(Session["pollIndex"]);
        }
        set
        {
            Session["pollIndex"] = value;
        }
    }

    protected ResultsList resultsList
    {
        get
        {
            return (ResultsList)Session["resultsList"];
        }
        set
        {
            Session["resultsList"] = value;
        }
    }

    public int SurveyID
    {
        get
        {
            return Convert.ToInt32(Session["surveyID"]);
        }
        set
        {
            Session["surveyID"] = value;
            
            resultsList = new ResultsList();
            resultsList.userName = Context.User.Identity.Name;
            resultsList.surveyId = value;
            
            pollIndex = 0;
        }
    }

    public int currentPollID
    {
        get
        {
            return Convert.ToInt32(survey.Polls[pollIndex].Id);
        }
    }

    public List<Poll> GetPolls()
    {
        return survey.Polls;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        surveyName.Text = survey.Name;

        if (!IsPostBack)
        {
            LoadData();
        }
    }

    protected void LoadData()
    {
        choiceList.DataSource = survey.Polls[pollIndex].Choices;
        choiceList.DataTextField = "choice";
        choiceList.DataValueField = "Id";
        choiceList.DataBind();

        pollName.Text = survey.Polls[pollIndex].Name;
        pollDesc.Text = survey.Polls[pollIndex].Description;

    }   
    
    protected void btnSurvey_Click(object sender, EventArgs e)
    {
        if (choiceList.SelectedIndex > -1)
        {
            PollResult pollResult = new PollResult();
            pollResult.questionId = survey.Polls[pollIndex].Id;
            pollResult.answerId = Convert.ToInt32(choiceList.SelectedValue);

            resultsList.results.Add(pollResult);

            ++pollIndex;

            if (pollIndex == survey.Polls.Count)
            {
                Response.Redirect("Results.aspx");
            }

            LoadData();

            if (pollIndex == survey.Polls.Count - 1)
            {
                btnSurvey.Text = "Finish";
            }
        }
        else
        {
            errorMessage.Text = "Please select a choice";
        }
    }
}
