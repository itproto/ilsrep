using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.DAL;

public partial class Results : System.Web.UI.Page
{
    protected bool passedSurvey;

    protected void Page_Load(object sender, EventArgs e)
    {
        SurveyResults results = Session["SurveyResults"] as SurveyResults;
        Survey survey = Session["survey"] as Survey;

        if ( survey == null || results == null )
            Response.Redirect( "Default.aspx" );

        DataTable userResults = new DataTable();
        userResults.Columns.Add( "poll" );
        userResults.Columns.Add( "choice" );

        int totalPoints = results.results.Count, userPoints = 0;
        foreach(PollResult result in results.results)
        {
            Poll currentPoll = survey.Polls.Find( delegate(Poll poll) { return poll.Id == result.questionId; } );

            userResults.Rows.Add( currentPoll.Name, currentPoll.Choices.Find( delegate( Choice choice ) { return choice.Id == result.answerId; } ).choice );

            if ( currentPoll.CorrectChoiceID == result.answerId )
                ++userPoints;
        }

        lblScore.Text = (userPoints*100/totalPoints).ToString();

        passedSurvey = (userPoints*100/totalPoints) >= survey.MinScore;
        
        surveyResults.DataSource = userResults;
        surveyResults.DataBind();
    }
}
