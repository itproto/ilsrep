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

            
                LiteralControl contentLiteralControl = new LiteralControl();
                contentLiteralControl.Text = "<table cellpadding='0' cellspacing='0' class='statistics_table'>";

                if (PollDAL.HasResults(surveyID))
                {
                    contentLiteralControl.Text += "<tr class='statistics_title'><td colspan='4'>" + curSurveyName + "</td></tr>";
                    contentLiteralControl.Text += "<tr class='statistics_title'><td>#</td><td>Users</td><td>Scores</td><td>Count of attempts</td></tr>";
                    List<String> users = new List<String>();
                    users = PollDAL.GetUsers();

                    int userIndex = 0;
                    foreach (String user in users)
                    {
                        userIndex++;
                        List<String> datesOfAttempts = new List<string>();
                        datesOfAttempts = PollDAL.GetDatesOfAttempts(user, surveyID);
                        if (datesOfAttempts.Count != 0)
                        {
                            contentLiteralControl.Text += "<tr><td>" + userIndex + "</td><td>" + user + "</td>";
                            contentLiteralControl.Text += "<td>HERE will be count of scores:)</td>";
                            contentLiteralControl.Text += "<td>" + datesOfAttempts.Count() + "</td></tr>";
                        }
                    }
                }
                else
                {
                    contentLiteralControl.Text += "<tr><td colspan='4'><h3>Sorry, this survey had no results</h3></td></tr>";
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
