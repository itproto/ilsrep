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
    //public SQLiteConnection sqliteConnection = new SQLiteConnection();
    public List<Item> testSurveysList = new List<Item>();
    public String surveyName;
    public String scoresDistribution = String.Empty;
    public String usersDistribution = String.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        //PollDAL.connectionString = "Data Source=\"" + Server.MapPath(ConfigurationSettings.AppSettings["dataSource"].ToString()) + "\"";

        // Left menu generating
        surveyMenu.DataSource = PollDAL.GetTestSurveys();
        surveyMenu.DataBind();

        if (Request["id"] != null)
        {
            try
            {
                int surveyID = Convert.ToInt32(Request["id"]);
                Survey survey = new Survey();
                survey = PollDAL.GetSurvey(surveyID);
                surveyName = survey.Name;

                // Add survey title to table
                TableRow titleRow = new TableRow();
                titleRow.CssClass = "statistics_title";
                TableCell titleCell = new TableCell();
                titleCell.ColumnSpan = 4;
                titleCell.Font.Bold = true;
                titleCell.Text = surveyName;
                titleRow.Cells.Add(titleCell);
                statisticsTable.Rows.Add(titleRow);

                if (PollDAL.HasResults(surveyID))
                {
                    List<String> users = new List<String>();
                    users = PollDAL.GetUsers();

                    List<StatisticsItem> statisticsItems = new List<StatisticsItem>();

                    // Form statisticsItems list
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

                    // Add titles of columns to table
                    TableRow tableRow = new TableRow();
                    tableRow.CssClass = "statistics_title";
                    TableCell indexCell = new TableCell();
                    TableCell userNameCell = new TableCell();
                    TableCell scoresCell = new TableCell();
                    TableCell attemptsCell = new TableCell();
                    indexCell.Text = "#";
                    userNameCell.Text = "Users";
                    scoresCell.Text = "Scores";
                    attemptsCell.Text = "Count of attempts";
                    tableRow.Cells.Add(indexCell);
                    tableRow.Cells.Add(userNameCell);
                    tableRow.Cells.Add(scoresCell);
                    tableRow.Cells.Add(attemptsCell);
                    statisticsTable.Rows.Add(tableRow);

                    int userIndex = 0;
                    foreach (StatisticsItem curItem in statisticsItems)
                    {
                        userIndex++;

                        bool addSeparator = (userIndex == statisticsItems.Count()) ? false : true;

                        // Form chart request
                        usersDistribution += curItem.userName + ((addSeparator) ? "|" : String.Empty);
                        scoresDistribution += Math.Round(curItem.scores) + ((addSeparator) ? "," : String.Empty);

                        tableRow = new TableRow();
                        indexCell = new TableCell();
                        userNameCell = new TableCell();
                        scoresCell = new TableCell();
                        attemptsCell = new TableCell();
                        indexCell.Text = userIndex.ToString();
                        userNameCell.Text = curItem.userName;
                        scoresCell.Text = String.Format("{0:G4}%", curItem.scores);
                        attemptsCell.Text = curItem.attemptsCount.ToString();
                        tableRow.Cells.Add(indexCell);
                        tableRow.Cells.Add(userNameCell);
                        tableRow.Cells.Add(scoresCell);
                        tableRow.Cells.Add(attemptsCell);
                        statisticsTable.Rows.Add(tableRow);
                    }
                    chart.ImageUrl = "http://chart.apis.google.com/chart?chco=d4d0b6&chxt=y&chs=500x100&chd=t:" + scoresDistribution + "&cht=bvs&chl=" + usersDistribution;
                }
                else
                {
                    TableRow messageRow = new TableRow();
                    messageRow.CssClass = "statistics_title";
                    TableCell messageCell = new TableCell();
                    messageCell.ColumnSpan = 4;
                    messageCell.Text = "<b>Sorry, this survey haven't results</b>";
                    messageRow.Cells.Add(messageCell);
                    statisticsTable.Rows.Add(messageRow);
                }
            }
            catch (Exception exception)
            {

            }
        }
        

    }    
}
