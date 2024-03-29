﻿using System;
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
using Ilsrep.PollApplication.Model;
using System.Collections.Generic;
using Ilsrep.PollApplication.DAL;

public partial class Widget : System.Web.UI.Page
{
    private int answerID = 0;
    private static int choiceID = 0;
    public static List<Choice> choices;
    public bool enableButtons;
    public int pollID = 0;

    public int GenerateAnswerID()
    {
        return ++answerID;
    }

    public int GenerateChoiceID()
    {
        return ++choiceID;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            choices = new List<Choice>();
            Choice choice1 = new Choice("answer1");
            Choice choice2 = new Choice("answer2");
            choice1.Id = GenerateChoiceID();
            choice2.Id = GenerateChoiceID();
            choices.Add(choice1);
            choices.Add(choice2);
            BindData();
            enableButtons = false;
        }
    }

    private void SaveChanges()
    {
        int index = 0;
        foreach (ListViewDataItem curItem in poll.Items)
        {
            choices[index++].choice = ((TextBox)curItem.FindControl("answerTextBox")).Text;
        }
    }

    private void BindData()
    {
        poll.DataSource = choices;
        poll.DataBind();
    }

    protected void OnItemCommandOccur(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            SaveChanges();
            switch (e.CommandName)
            {
                case "AddItem":
                    Choice choice = new Choice(((TextBox)poll.FindControl("newAnswerTextBox")).Text);
                    choice.Id = GenerateChoiceID();
                    ((TextBox)poll.FindControl("newAnswerTextBox")).Text = String.Empty;
                    choices.Add(choice);
                    enableButtons = (choices.Count > 2) ? true : false;
                    BindData();                   
                    break;
                case "RemoveItem":                    
                    choices.Remove(choices.Find(delegate(Choice curChoice) { return curChoice.Id == Convert.ToInt32(e.CommandArgument); }));
                    enableButtons = (choices.Count > 2) ? true : false;
                    BindData();                    
                    break;
                case "SubmitWidget":
                    string question = ((TextBox)poll.FindControl("questionTextBox")).Text;
                    if (question == String.Empty)
                    {
                        throw new Exception("Question field can't be empty");
                    }

                    Poll newPoll = new Poll();
                    newPoll.Description = question;
                    newPoll.Choices = choices;
                    newPoll.Name = "WidgetPoll";

                    pollID = PollDAL.CreatePoll(newPoll);

                    resultWidget.Visible = true;
                    widgetSrc.InnerText = "<iframe style=\"border: solid 1px #A30313; width: 320px; height: 165px\" scrolling=\"auto\" src=\"PollWidget.aspx?poll_id=" + pollID.ToString() + "\"></iframe>";

                    break;
            }
        }
        catch (Exception exception)
        {
            errorLabel.Text = exception.Message;
        }
    }
}
