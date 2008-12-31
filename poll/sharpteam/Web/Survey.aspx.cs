using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ilsrep.PollApplication.DAL;
using Ilsrep.PollApplication.Model;

public partial class _Survey : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request["id"] == null)
                Response.Redirect("Default.aspx");

            surveyControl.SurveyID = Convert.ToInt32(Request["id"]);
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        pollMenu.DataSource = surveyControl.GetPolls();
        pollMenu.DataBind();
    }

}