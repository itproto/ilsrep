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
    public String surveyName = String.Empty;
    public String userName = String.Empty;
    public String scoresDistribution = String.Empty;
    public String labelsDistribution = String.Empty;
    public List<StatisticsItem> statisticsItems = new List<StatisticsItem>();

    private void GenerateSurveysTable(int startIndex, int endIndex)
    {
        // Add survey title to table
        TableRow titleRow = new TableRow();
        titleRow.CssClass = "statistics_title";
        TableCell titleCell = new TableCell();
        titleCell.ColumnSpan = 4;
        titleCell.Font.Bold = true;
        titleCell.Text = surveyName;
        titleRow.Cells.Add(titleCell);
        statisticsTable.Rows.Add(titleRow);

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

        for (int index = startIndex; index <= endIndex; index++)
        {            
            bool addSeparator = (index == endIndex) ? false : true;

            // Form chart request
            labelsDistribution += statisticsItems[index].name + ((addSeparator) ? "|" : String.Empty);
            scoresDistribution += Math.Round(statisticsItems[index].scores) + ((addSeparator) ? "," : String.Empty);

            tableRow = new TableRow();
            indexCell = new TableCell();
            userNameCell = new TableCell();
            scoresCell = new TableCell();
            attemptsCell = new TableCell();
            indexCell.Text = (index + 1).ToString();
            userNameCell.Text = "<a href='Statistics.aspx?object=user&name=" + statisticsItems[index].name + "'>" + statisticsItems[index].name + "</a>";
            scoresCell.Text = String.Format("{0:G4}%", statisticsItems[index].scores);
            attemptsCell.Text = statisticsItems[index].attemptsCount.ToString();
            tableRow.Cells.Add(indexCell);
            tableRow.Cells.Add(userNameCell);
            tableRow.Cells.Add(scoresCell);
            tableRow.Cells.Add(attemptsCell);
            statisticsTable.Rows.Add(tableRow);
        }
    }

    private void GenerateUsersTable(int startIndex, int endIndex)
    {
        // Add user name to table
        TableRow titleRow = new TableRow();
        titleRow.CssClass = "statistics_title";
        TableCell titleCell = new TableCell();
        titleCell.ColumnSpan = 4;
        titleCell.Font.Bold = true;
        titleCell.Text = userName;
        titleRow.Cells.Add(titleCell);
        statisticsTable.Rows.Add(titleRow);

        // Add titles of columns to table
        TableRow tableRow = new TableRow();
        tableRow.CssClass = "statistics_title";
        TableCell indexCell = new TableCell();
        TableCell surveyNameCell = new TableCell();
        TableCell scoresCell = new TableCell();
        TableCell attemptsCell = new TableCell();
        indexCell.Text = "#";
        surveyNameCell.Text = "Surveys";
        scoresCell.Text = "Scores";
        attemptsCell.Text = "Count of attempts";
        tableRow.Cells.Add(indexCell);
        tableRow.Cells.Add(surveyNameCell);
        tableRow.Cells.Add(scoresCell);
        tableRow.Cells.Add(attemptsCell);
        statisticsTable.Rows.Add(tableRow);

        for (int index = startIndex; index <= endIndex; index++)
        {
            bool addSeparator = (index == endIndex) ? false : true;

            // Form chart request
            labelsDistribution += statisticsItems[index].name + ((addSeparator) ? "|" : String.Empty);
            scoresDistribution += Math.Round(statisticsItems[index].scores) + ((addSeparator) ? "," : String.Empty);

            tableRow = new TableRow();
            indexCell = new TableCell();
            surveyNameCell = new TableCell();
            scoresCell = new TableCell();
            attemptsCell = new TableCell();
            indexCell.Text = (index + 1).ToString();
            surveyNameCell.Text = statisticsItems[index].name;
            scoresCell.Text = String.Format("{0:G4}%", statisticsItems[index].scores);
            attemptsCell.Text = statisticsItems[index].attemptsCount.ToString();
            tableRow.Cells.Add(indexCell);
            tableRow.Cells.Add(surveyNameCell);
            tableRow.Cells.Add(scoresCell);
            tableRow.Cells.Add(attemptsCell);
            statisticsTable.Rows.Add(tableRow);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Left menu generating
        surveyMenu.DataSource = PollDAL.GetTestSurveys();
        surveyMenu.DataBind();
        
        #region Show statistics

        switch (Request["object"])
        {
            case "survey":
                #region Show survey statistics

                if (Request["id"] != null)
                {
                    try
                    {
                        int surveyID = Convert.ToInt32(Request["id"]);
                        Survey survey = new Survey();
                        survey = PollDAL.GetSurvey(surveyID);
                        surveyName = survey.Name;

                        if (PollDAL.HasResults(surveyID))
                        {
                            List<String> users = new List<String>();
                            users = PollDAL.GetUsers();

                            #region Form statisticsItems list

                            foreach (String user in users)
                            {
                                List<DateTime> datesOfAttempts = new List<DateTime>();
                                datesOfAttempts = PollDAL.GetDatesOfAttempts(user, surveyID);

                                if (datesOfAttempts.Count != 0)
                                {
                                    StatisticsItem curStatisticsItem = new StatisticsItem();
                                    curStatisticsItem.name = user;
                                    curStatisticsItem.attemptsCount = datesOfAttempts.Count();

                                    // Calculate count of scores
                                    List<Double> scoresOfAttempts = new List<Double>();
                                    
                                    foreach (DateTime curDate in datesOfAttempts)
                                    {
                                        SurveyResults resultsList = new SurveyResults();
                                        resultsList = PollDAL.GetSurveyResults(surveyID, user, curDate);

                                        int countOfCorrectAnswers = 0;

                                        foreach (PollResult curResult in resultsList.results)
                                        {
                                            Poll currentPoll = survey.Polls.Find(delegate(Poll poll) { return poll.Id == curResult.questionId; });
                                            if (currentPoll.CorrectChoiceID == curResult.answerId)
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

                            #endregion

                            // Sort by scores
                            statisticsItems.Sort();

                            // Generate pages
                            int pageCount = Convert.ToInt32(Math.Truncate(Convert.ToDecimal((statisticsItems.Count - 1) / 10 + 1)));
                            int curPage = 1;

                            if (Request["page"] != null) 
                                curPage = Convert.ToInt32(Request["page"]);

                            if (statisticsItems.Count <= 10)
                            {
                                pagesLabel.Text = "Pages: 1";
                            }
                            else if (statisticsItems.Count == 0)
                            {
                                pagesLabel.Text = "No data to display";
                            }
                            else
                            {
                                pagesLabel.Text = "Pages: ";
                                for (int i = 1; i <= pageCount; i++)
                                {
                                    if (i == curPage)
                                        pagesLabel.Text += " " + i.ToString();
                                    else
                                        pagesLabel.Text += " <a href='Statistics.aspx?object=survey&id=" + Request["id"] + "&page=" + i.ToString() + "'>" + i.ToString() + "</a>";
                                }
                            }

                            // Generate table
                            if (statisticsItems.Count > 10)
                            {
                                if (Request["page"] == null)
                                {
                                    GenerateSurveysTable(0, 9);
                                }
                                else
                                {
                                    int page = Convert.ToInt32(Request["page"]);
                                    if ((page > pageCount) || (page <= 0))
                                        throw new Exception("Invalid page");
                                    
                                    int beginIndex = (page - 1) * 10;
                                    int endIndex = (((statisticsItems.Count - beginIndex) > 9) ? (page * 10 - 1) : (statisticsItems.Count - 1));
                                    GenerateSurveysTable(beginIndex, endIndex);                                 
                                }
                            }
                            else
                            {
                                GenerateSurveysTable(0, statisticsItems.Count - 1);
                            }

                            chart.ImageUrl = "http://chart.apis.google.com/chart?chco=d4d0b6&chbh=40&chxt=y&chs=500x300&chd=t:" + scoresDistribution + "&cht=bvs&chl=" + labelsDistribution;
                        }
                        else
                        {
                            throw new Exception("Sorry, this survey haven't results yet");
                        }
                    }
                    catch (Exception exception)
                    {
                        message.Text = exception.Message;
                    }
                }

                #endregion
                break;
            case "user":
                #region Show user statistics

                try
                {
                    userName = Request["name"];

                    if (PollDAL.HasResults(userName))
                    {
                        List<Item> surveys = new List<Item>();
                        surveys = PollDAL.GetTestSurveys();

                        #region Form statisticsItems list

                        foreach (Item surveyItem in surveys)
                        {
                            Survey survey = PollDAL.GetSurvey(surveyItem.id);
                            StatisticsItem curStatisticsItem = new StatisticsItem();
                            curStatisticsItem.name = survey.Name;
                            List<DateTime> datesOfAttempts = new List<DateTime>();
                            datesOfAttempts = PollDAL.GetDatesOfAttempts(userName, survey.Id);
                            curStatisticsItem.attemptsCount = datesOfAttempts.Count();
                            
                            if (datesOfAttempts.Count != 0)
                            {
                                // Calculate count of scores
                                List<Double> scoresOfAttempts = new List<Double>();
                                foreach (DateTime curDate in datesOfAttempts)
                                {
                                    SurveyResults resultsList = new SurveyResults();
                                    resultsList = PollDAL.GetSurveyResults(survey.Id, userName, curDate);

                                    int countOfCorrectAnswers = 0;
                                    foreach (PollResult curResult in resultsList.results)
                                    {
                                        Poll currentPoll = survey.Polls.Find(delegate(Poll poll) { return poll.Id == curResult.questionId; });
                                        if (currentPoll.CorrectChoiceID == curResult.answerId)
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

                        #endregion

                        // Sort by scores
                        statisticsItems.Sort();

                        // Generate pages
                        int pageCount = Convert.ToInt32(Math.Truncate(Convert.ToDecimal((statisticsItems.Count - 1) / 10 + 1)));
                        int curPage = 1;

                        if (Request["page"] != null)
                            curPage = Convert.ToInt32(Request["page"]);

                        if (statisticsItems.Count <= 10)
                        {
                            pagesLabel.Text = "Pages: 1";
                        }
                        else if (statisticsItems.Count == 0)
                        {
                            pagesLabel.Text = "No data to display";
                        }
                        else
                        {
                            pagesLabel.Text = "Pages: ";
                            for (int i = 1; i <= pageCount; i++)
                            {
                                if (i == curPage)
                                    pagesLabel.Text += " " + i.ToString();
                                else
                                    pagesLabel.Text += " <a href='Statistics.aspx?object=user&name=" + Request["name"] + "&page=" + i.ToString() + "'>" + i.ToString() + "</a>";
                            }
                        }

                        if (statisticsItems.Count > 10)
                        {
                            if (Request["page"] == null)
                            {
                                GenerateUsersTable(0, 9);
                            }
                            else
                            {
                                int page = Convert.ToInt32(Request["page"]);
                                if (page > pageCount)
                                    throw new Exception("Invalid page");

                                int beginIndex = (page - 1) * 10;
                                int endIndex = (((statisticsItems.Count - beginIndex) > 9) ? (page * 10 - 1) : (statisticsItems.Count - 1));
                                GenerateUsersTable(beginIndex, endIndex);
                            }
                        }
                        else
                        {
                            GenerateUsersTable(0, statisticsItems.Count - 1);
                        }

                        chart.ImageUrl = "http://chart.apis.google.com/chart?chco=d4d0b6&chbh=40&chxt=y&chs=500x300&chd=t:" + scoresDistribution + "&cht=bvs&chl=" + labelsDistribution;
                    }
                    else
                    {
                        throw new Exception("Sorry, this user haven't results yet");
                    }
                }
                catch (Exception exception)
                {
                    message.Text = exception.Message;
                }

                #endregion
                break;
            default:
                message.Text = "Please select survey from left menu.";
                break;
        }

        #endregion

    }    
}
