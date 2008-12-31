using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ilsrep.PollApplication.Communication;
using Ilsrep.PollApplication.Model;
using Ilsrep.PollApplication.Helpers;
using Ilsrep.PollApplication.DAL;
using System.Configuration;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        PollDAL.connectionString = "Data Source=\"" + Server.MapPath(ConfigurationSettings.AppSettings["dataSource"].ToString()) + "\"";
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
    }
}
