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

namespace KioskBrowser
{
    public partial class _Default : System.Web.UI.Page
    {
        private const string ADMIN_PASS = "1";
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public void okButton_Click(object sender, EventArgs e)
        {
            if (passTextBox.Text == ADMIN_PASS)
            {
                Response.Redirect("AdminPage.aspx");
            }
            else
            {
                Response.Redirect("ClientPage.aspx");
            }
        }
    }
}
