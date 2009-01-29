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
    protected void Page_Load( object sender, EventArgs e )
    {
        surveyMenu.DataSource = PollDAL.GetSurveys();
        surveyMenu.DataBind();
    }

    public void LogOut()
    {
        FormsAuthentication.SignOut();
    }
}