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
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Communication;
using System.Collections.Generic;

public partial class Statistics : System.Web.UI.Page
{
    public List<StatisticsItem> statisticsItems;
    public string tableTitle;
    public string objectName;
    public string objectInLink;
    public string idInLink;
    public string idValueInLink;
    private int index;

    public void StatisticsPreRender(object sender, EventArgs e)
    {
        index = statisticsDataPager.StartRowIndex;
        statisticsTableList.DataSource = statisticsItems;
        statisticsTableList.DataBind();
        if (statisticsItems != null)
        {
            FormChart();
        }

        if (Request["object"] == "survey")
        {
            objectInLink = "user";
            idInLink = "name";
        }
        else if (Request["object"] == "user")
        {
            objectInLink = "survey";
            idInLink = "id";
        }
    }

    public bool IsSelected(string ID)
    {
        if (Request["object"] == "survey" && Request["id"] == ID)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FormChart()
    {
        // Form chart request
        string labelsDistribution = String.Empty;
        string scoresDistribution = String.Empty;

        int startIndex = statisticsDataPager.StartRowIndex;
        bool isLastPage = ((statisticsDataPager.TotalRowCount - statisticsDataPager.StartRowIndex) < statisticsDataPager.MaximumRows);
        int endIndex = (isLastPage) ? statisticsDataPager.TotalRowCount : startIndex + statisticsDataPager.MaximumRows;

        for (int i = startIndex; i < endIndex; i++)
        {
            bool addSeparator = (i != endIndex - 1);
            labelsDistribution += statisticsItems[i].name + ((addSeparator) ? "|" : String.Empty);
            scoresDistribution += Math.Round(statisticsItems[i].scores) + ((addSeparator) ? "," : String.Empty);
        }

        chart.ImageUrl = "http://chart.apis.google.com/chart?chco=A30313&chbh=40&chxt=y&chs=500x300&chd=t:" + scoresDistribution + "&cht=bvs&chl=" + labelsDistribution;
    }

    public int GetIndex()
    {
        return ++index;
    }

    public List<StatisticsItem> FormSurveyStatisticsList()
    {
        List<StatisticsItem> statisticsItems = new List<StatisticsItem>();
        List<String> users = PollDAL.GetUsers();
        int surveyID = Convert.ToInt32(Request["id"]);
        Survey survey = PollDAL.GetSurvey(surveyID);
        foreach (String user in users)
        {
            List<DateTime> datesOfAttempts = new List<DateTime>();
            datesOfAttempts = PollDAL.GetDatesOfAttempts(user, surveyID);

            if (datesOfAttempts.Count != 0)
            {
                StatisticsItem curStatisticsItem = new StatisticsItem();
                curStatisticsItem.name = user;
                curStatisticsItem.ID = user;
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

        // Sort by scores
        statisticsItems.Sort();

        return statisticsItems;
    }

    public List<StatisticsItem> FormUserStatisticsList()
    {
        string userName = Request["name"]; 
        List<StatisticsItem> statisticsItems = new List<StatisticsItem>();
        List<Item> surveys = PollDAL.GetTestSurveys();
        foreach (Item surveyItem in surveys)
        {
            Survey survey = PollDAL.GetSurvey(surveyItem.id);
            StatisticsItem curStatisticsItem = new StatisticsItem();
            curStatisticsItem.name = survey.Name;
            curStatisticsItem.ID = survey.Id.ToString();
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

        // Sort by scores
        statisticsItems.Sort();

        return statisticsItems;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        surveyMenu.DataSource = PollDAL.GetTestSurveys();
        surveyMenu.DataBind();        

        try
        {
            switch (Request["object"])
            {
                case "survey":
                    statisticsItems = FormSurveyStatisticsList();
                    int surveyID = Convert.ToInt32(Request["id"]);
                    Survey survey = PollDAL.GetSurvey(surveyID);                   
                    if (PollDAL.HasResults(surveyID))
                    {
                        tableTitle = survey.Name;
                        objectName = "Users";
                    }
                    break;
                case "user":
                    statisticsItems = FormUserStatisticsList();
                    string userName = Request["name"];
                    if (PollDAL.HasResults(userName))
                    {
                        tableTitle = userName;
                        objectName = "Surveys";
                    }
                    break;
                default:
                    message.Text = "Please select survey from left menu.";
                    break;
            }
        }
        catch (Exception exception)
        {
            message.Text = exception.Message;
        }
    }
}
