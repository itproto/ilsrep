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

        String returnUrl = String.Empty;
        if (Request["ReturnUrl"] != null)
        {
            returnUrl = Request["ReturnUrl"];
        }

        if (returnUrl.IndexOf("PollEditor.aspx") > -1)
        {
            Response.Redirect("messages.aspx?type=deny_access&redirect=Default.aspx");
        }
    }

    public void Register_Click(Object sender, EventArgs e)
    {
        if ((regUsername.Text == String.Empty) || (regPassword.Text == String.Empty) || (regConfirmPassword.Text == String.Empty))
        {
            regMessage.InnerHtml = "Please, fill all fields";
            return;
        }

        if (regPassword.Text != regConfirmPassword.Text)
        {
            regMessage.InnerHtml = "Password and Confirm password fields must be the same";
            return;
        }

        Ilsrep.PollApplication.Model.User user = new Ilsrep.PollApplication.Model.User();
        user.username = regUsername.Text;
        user.password = regPassword.Text;

        user = PollDAL.ExistUser(user);

        if (user.action == Ilsrep.PollApplication.Model.User.NEW_USER)
        {
            user = PollDAL.RegisterUser(user);
            if (user.action == Ilsrep.PollApplication.Model.User.AUTH)
            {
                FormsAuthentication.RedirectFromLoginPage(regUsername.Text, false);
            }
            else
            {
                regMessage.InnerHtml = "An undefined error occured in server";
            }
        }
        else
        {
            regMessage.InnerHtml = "Such user already exists";
        }
    }

    public void Login_Click(Object sender, EventArgs e)
    {
        Ilsrep.PollApplication.Model.User user = new Ilsrep.PollApplication.Model.User();
        user.username = username.Text;
        user.password = password.Text;
        user = PollDAL.AuthorizeUser(user);

        if (user.action == Ilsrep.PollApplication.Model.User.AUTH)
            FormsAuthentication.RedirectFromLoginPage(username.Text, false);
        else
            message.InnerHtml = "Invalid credentials!";
    }
}
