using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Ilsrep.PollApplication.DAL;
using System.Data.SQLite;
using System.Web.UI.MobileControls;
using Ilsrep.PollApplication.Communication;
using System.Collections.Generic;
using Ilsrep.PollApplication.Model;


public partial class Statistics : System.Web.UI.Page
{
    public SQLiteConnection sqliteConnection = new SQLiteConnection();
    public List<Item> testSurveysList = new List<Item>();
    public String curSurveyName;

    protected void Page_Load(object sender, EventArgs e)
    {
        PollDAL.connectionString = "Data Source=\"" + Server.MapPath(ConfigurationSettings.AppSettings["dataSource"].ToString()) + "\"";
        testSurveysList = PollDAL.GetTestSurveys();

        // Left menu generating
        leftMenuPanel.Controls.Add(new LiteralControl("<ul>"));
        LiteralControl menuLiteralControl = new LiteralControl();
        menuLiteralControl.Text = "<h3>Select Survey :</h3>";
        int index = 1;
        foreach (Item curItem in testSurveysList)
        {
            if (Request["id"] == curItem.id.ToString())
            {
                menuLiteralControl.Text += "<li>" + index + ". " + curItem.name + "</li>";
                curSurveyName = curItem.name;
            }
            else
            {
                menuLiteralControl.Text += "<li><a href='Statistics.aspx?action=showstatistics&id=" + curItem.id + "' onfocus='this.blur()'>" + index + ". " + curItem.name + "</a></li>";
            }
            index++;
        }
        leftMenuPanel.Controls.Add(menuLiteralControl);
        leftMenuPanel.Controls.Add(new LiteralControl("</ul>"));

        if (Request["id"] != null)
        {
            try
            {
                int surveyID = Convert.ToInt32(Request["id"]);
                Survey survey = new Survey();
                survey = PollDAL.GetSurvey(surveyID);
            
                LiteralControl contentLiteralControl = new LiteralControl();
                contentLiteralControl.Text = "<table cellpadding='0' cellspacing='0' class='statistics_table'>";

                if (PollDAL.HasResults(surveyID))
                {
                    contentLiteralControl.Text += "<tr class='statistics_title'><td colspan='4'>" + curSurveyName + "</td></tr>";
                    contentLiteralControl.Text += "<tr class='statistics_title'><td>#</td><td>Users</td><td>Scores</td><td>Count of attempts</td></tr>";
                    List<String> users = new List<String>();
                    users = PollDAL.GetUsers();

                    List<StatisticsItem> statisticsItems = new List<StatisticsItem>();
                    foreach (String user in users)
                    {
                        StatisticsItem curStatisticsItem = new StatisticsItem();
                        curStatisticsItem.userName = user;
                        List<DateTime> datesOfAttempts = new List<DateTime>();
                        datesOfAttempts = PollDAL.GetDatesOfAttempts(user, surveyID);
                        curStatisticsItem.attemptsCount = datesOfAttempts.Count();
                        if (datesOfAttempts.Count != 0)
                        {
                            // Calculate count of scores
                            List<Double> scoresOfAttempts = new List<Double>();
                            foreach (DateTime curDate in datesOfAttempts)
                            {
                                ResultsList resultsList = new ResultsList();
                                resultsList = PollDAL.GetSurveyResults(surveyID, user, curDate);

                                int countOfCorrectAnswers = 0;
                                foreach (PollResult curResult in resultsList.results)
                                {
                                    if (survey.Polls[curResult.questionId].Choices[curResult.answerId].Id == survey.Polls[curResult.questionId].CorrectChoiceID)
                                        countOfCorrectAnswers++;
                                }
                                Double curScores = Convert.ToDouble(countOfCorrectAnswers) / Convert.ToDouble(resultsList.results.Count);
                                scoresOfAttempts.Add(curScores);
                            }

                            Double scores = 0;
                            foreach (Double curScores in scoresOfAttempts)
                            {
                                scores += curScores;
                            }
                            scores /= Convert.ToDouble(datesOfAttempts.Count) / 100;
                            curStatisticsItem.scores = scores;
                            statisticsItems.Add(curStatisticsItem);
                        }
                    }

                    // Sort by scores
                    statisticsItems.Sort();

                    int userIndex = 0;
                    foreach (StatisticsItem curItem in statisticsItems)
                    {
                        userIndex++;
                        contentLiteralControl.Text += "<tr><td>" + userIndex + "</td><td>" + curItem.userName + "</td>";
                        contentLiteralControl.Text += String.Format("<td>{0:G4} %</td>", curItem.scores);
                        contentLiteralControl.Text += "<td>" + curItem.attemptsCount + "</td></tr>";
                    }

                }
                else
                {
                    contentLiteralControl.Text += "<tr><td colspan='4'><h3>Sorry, this survey haven't results</h3></td></tr>";
                }

                contentLiteralControl.Text += "</table>";
                contentPanel.Controls.Add(contentLiteralControl);
            }
            catch (Exception exception)
            {

            }
        }
    }
}
