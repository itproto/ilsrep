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
        PollDAL.connectionString = "Data Source=\"" + Server.MapPath(ConfigurationSettings.AppSettings["dataSource"].ToString()) + "\"";
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
                resultsList.userName = User.Identity.Name;
                resultsList.surveyId = survey.Id;
                PollDAL.SaveSurveyResult(resultsList);
                break;
        }

       // Left menu generating
        /*leftMenuPanel.Controls.Add(new LiteralControl("<ul>"));
        int index;
        if ((Request["action"] == "showsurvey") || (Request["action"] == "submitpoll"))
        {
            //LiteralControl literalControl = new LiteralControl();
            //literalControl.Text = "<li><h3>" + survey.Name + ":</h3></li>";
            index = 1;
            foreach (Ilsrep.PollApplication.Model.Poll curPoll in survey.Polls)
            {
                if (Convert.ToInt32(Session["pollIndex"]) == index - 1)
                {
                    literalControl.Text += "<li><b>" + index.ToString() + ". " + curPoll.Name + "</b></li>";
                }
                else
                {
                    literalControl.Text += "<li>" + index.ToString() + ". " + curPoll.Name + "</li>";
                }
                index++;
            }
            leftMenuPanel.Controls.Add(literalControl);
        }
        else
        {
            LiteralControl literalControl1 = new LiteralControl();
            literalControl1.Text = "<h3>Select Survey :</h3>";
            leftMenuPanel.Controls.Add(literalControl1);
            index = 1;
            foreach (Ilsrep.PollApplication.Communication.Item curItem in surveysList)
            {
                LiteralControl literalControl2 = new LiteralControl();
                literalControl2.Text = "<li><a href='Default.aspx?action=showsurvey&id=" + curItem.id + "' onfocus='this.blur()'>" + index + ". " + curItem.name + "</a></li>";
                leftMenuPanel.Controls.Add(literalControl2);
                index++;
            }
        }
        leftMenuPanel.Controls.Add(new LiteralControl("</ul>"));
         * */

        pollMenu.DataSource = surveysList;
        pollMenu.DataBind();

        // Content generating
        /*
        if ((Request["action"] == "showsurvey") || (Request["action"] == "submitpoll"))
        {
            LiteralControl literalControl = new LiteralControl();
            literalControl.Text = "<form class='choices' method='post' action='Default.aspx?action=submitpoll'>";
            literalControl.Text += "<h3>" + survey.Polls[Convert.ToInt32(Session["pollIndex"])].Description + "</h3>";
            index = 0;
            foreach (Ilsrep.PollApplication.Model.Choice curChoice in survey.Polls[Convert.ToInt32(Session["pollIndex"])].Choices)
            {
                if (index == 0)
                {
                    literalControl.Text += "<label for='choice_" + index + "'><input checked='true' type='radio' onfocus='this.blur();' name='choice' id='choice_" + index + "' value='" + index + "' />" + curChoice.choice + "</label><br />";
                }
                else
                {
                    literalControl.Text += "<label for='choice_" + index + "'><input type='radio' onfocus='this.blur();' name='choice' id='choice_" + index + "' value='" + index + "' />" + curChoice.choice + "</label><br />";
                }
                index++;
            }
            literalControl.Text += "<input type='submit' class='submitButton' value='Continue' onfocus='this.blur();' onclick='return CheckIfSelectedChoice();' /></form>";
            contentPanel.Controls.Add(literalControl);
        }
        else if (Request["action"] == "showresults")
        {
            LiteralControl literalControl = new LiteralControl();
            literalControl.Text = "<div class='inner'>";
            literalControl.Text += "<h3>Here is your Survey results:<br /></h3>";
            float correctAnswers = 0;
            index = 0;
            foreach (Ilsrep.PollApplication.Model.PollResult curResult in resultsList.results)
            {
                index++;    
                literalControl.Text += index + ". " + survey.Polls[curResult.questionId].Name + ": " + survey.Polls[curResult.questionId].Choices[curResult.answerId].choice + "<br />";
                if (survey.TestMode)
                {
                    if (survey.Polls[curResult.questionId].Choices[curResult.answerId].Id == survey.Polls[curResult.questionId].CorrectChoiceID)
                        correctAnswers++;
                }
            }

            if (survey.TestMode)
            {
                double userScore = correctAnswers / survey.Polls.Count;
                literalControl.Text += "<br />Your scores: " + Convert.ToInt32(userScore * 100) + "%";
                if (userScore >= survey.MinScore)
                {
                    literalControl.Text += "<br /><br />Congratulations!!! You PASSED the test";
                }
                else
                {
                    literalControl.Text += "<br /><br />Sorry, try again... you FAILED";
                }
            }
            literalControl.Text += "</div>";
            contentPanel.Controls.Add(literalControl);
        }*/
    }

    public void LogOut()
    {
        FormsAuthentication.SignOut();
    }
}
