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
using System.Collections.Generic;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.DAL;

public partial class PollWidget : System.Web.UI.Page
{
    public Poll poll;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                int pollID = Convert.ToInt32(Request["poll_id"]);
                poll = PollDAL.GetPoll(pollID);
                BindData();
            }
        }
        catch (Exception exception)
        {
            errorLabel.Text = exception.Message;
        }
    }

    public void BindData()
    {
        titleLabel.Text = poll.Description;
        choicesRadioButtonList.Items.Clear();
        foreach (Choice curChoice in poll.Choices)
        {
            ListItem curItem = new ListItem(curChoice.choice, curChoice.Id.ToString());
            choicesRadioButtonList.Items.Add(curItem);
        }
    }

    public void SaveAnswer()
    {
        int pollID = Convert.ToInt32(Request["poll_id"]);
        PollResult pollResult = new PollResult();
        pollResult.questionId = Convert.ToInt32(pollID);
        pollResult.answerId = Convert.ToInt32(choicesRadioButtonList.SelectedValue);
        pollResult.userName = "unknown(<widget>)";
        PollDAL.SavePollResult(pollResult);
    }

    public void GetResults()
    {
        errorLabel.Visible = false;
        choicesRadioButtonList.Visible = false;
        submitButton.Visible = false;

        int pollID = Convert.ToInt32(Request["poll_id"]);
        List<PollResult> rawResults = PollDAL.GetPollResults(pollID);
        Poll curPoll = PollDAL.GetPoll(pollID);
        List<PollResultItem> resultsList = new List<PollResultItem>();

        foreach (Choice curChoice in curPoll.Choices)
        {
            PollResultItem newPollPollResultItem = new PollResultItem();
            newPollPollResultItem.ChoiceName = curChoice.choice;
            newPollPollResultItem.VotesCount = 0;
            resultsList.Add(newPollPollResultItem);
        }

        foreach (PollResult pollResult in rawResults)
        {
            string curAnswer = PollDAL.GetChoice(pollResult.answerId);
            PollResultItem curPollResultItem = resultsList.Find(delegate(PollResultItem pollResultItem) { return pollResultItem.ChoiceName == curAnswer; });

            if (curPollResultItem != null)
            {
                resultsList.Find(delegate(PollResultItem pollResultItem){return pollResultItem.ChoiceName == curAnswer;}).VotesCount++;
            }
        }

        foreach (PollResultItem pollResultItem in resultsList)
        {
            pollResultItem.Percentage = Convert.ToInt32((pollResultItem.VotesCount * 100) / rawResults.Count);
        }

        // Form chart
        string scoresDistribution = string.Empty;
        string answersDistribution = string.Empty;

        for (int i = 0, j = resultsList.Count - 1; i < resultsList.Count; i++, j--)
        {
            bool addSeparator = (j == 0) ? false : true;
            answersDistribution += resultsList[i].ChoiceName + "(" + resultsList[i].VotesCount + " votes)" + ((addSeparator) ? "|" : String.Empty);
            scoresDistribution += resultsList[j].Percentage.ToString() + ((addSeparator) ? "," : String.Empty);
        }

        int chartHeight = 40 + 20 * resultsList.Count;

        chart.ImageUrl = "http://chart.apis.google.com/chart?chbh=15,4,8&chco=A30313&chs=300x" + chartHeight.ToString() + "&chd=t:" + scoresDistribution + "&cht=bhs&chxt=y,x&chxl=0:|" + answersDistribution + "|1:|0%|20%|40%|60%|80%|100%";
    }

    protected void SubmitButtonClick(object sender, EventArgs e)
    {
        try
        {
            if (choicesRadioButtonList.SelectedIndex == -1)
            {
                throw new Exception("Please, select any choice");
            }
            else
            {
                SaveAnswer();
                GetResults();
            }
        }
        catch (Exception exception)
        {
            errorLabel.Text = exception.Message;
        }
    }

    private class PollResultItem
    {
        public string ChoiceName;
        public int VotesCount;
        public int Percentage;
    }
}
