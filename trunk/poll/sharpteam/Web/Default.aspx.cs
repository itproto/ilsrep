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
    public SQLiteConnection sqliteConnection = new SQLiteConnection();
    public string errorMessage = String.Empty;

    const string ERROR_AUTH = "Wrong username or password!";

    protected void Page_Load( object sender, EventArgs e )
    {
        PollDAL.connectionString = "Data Source=\"" + Server.MapPath( ConfigurationSettings.AppSettings["dataSource"].ToString() ) + "\"";

        if ( Request["do"] == "login" )
        {
            User user = new User();
            user.username = Request["username"];
            user.password = Request["password"];
            PollDAL.AuthorizeUser( user );

            if ( !user.auth )
            {
                errorMessage = ERROR_AUTH;
                return;
            }
            else
            {
                Session["loggedin"] = user.auth;
                Response.Redirect( "Main.aspx" );
            }
        }
    }
}
