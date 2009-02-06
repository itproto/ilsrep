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

public partial class PollWidget : System.Web.UI.Page
{
    public String question;
    public List<Choice> choices;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            question = "Question?";
            choices = new List<Choice>();
            Choice choice1 = new Choice("answer1");
            Choice choice2 = new Choice("answer2");
            choice1.Id = 0;
            choice2.Id = 1;
            choices.Add(choice1);
            choices.Add(choice2);
            BindData();
        }
    }

    public void BindData()
    {
        titleLabel.Text = question;
        choicesRadioButtonList.Items.Clear();
        foreach (Choice curChoice in choices)
        {
            ListItem curItem = new ListItem(curChoice.choice, curChoice.Id.ToString());
            choicesRadioButtonList.Items.Add(curItem);
        }
    }

    protected void SubmitButtonClick(object sender, EventArgs e)
    {
        titleLabel.Text = choicesRadioButtonList.SelectedValue;
    }
}
