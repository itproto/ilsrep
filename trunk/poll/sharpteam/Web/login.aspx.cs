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

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        switch (Request["action"])
        {
            case "logout":
                FormsAuthentication.SignOut();
                break;
        }

        if (Request["ReturnUrl"] == "/Web/PollEditor.aspx")
        {
            backButton.Visible = true;
            message.InnerText = "Sorry, this page is only for moderators";
        }
    }

    public void Login_Click(Object sender, EventArgs e)
    {
        if (true)
            FormsAuthentication.RedirectFromLoginPage(username.Text, false);
        else
            message.InnerHtml = "Invalid Login";
    }

    public void Back_Click(Object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
}
