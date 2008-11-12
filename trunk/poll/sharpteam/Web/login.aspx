<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="login.aspx.cs" Inherits="login" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head>
    <title>Login</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <link href="css/style.css" type="text/css" rel="Stylesheet" />
    <script src="js/scripts.js" type="text/javascript"></script>
</head>
<body class="login_body">
    <%
    switch (Request["action"])
    {
        case "registration":
            %>
            <div class="registration_main">
                <form id="registrationForm" runat="server">
                    <div class="login_title">Registration</div>
                    <div class="login_field">Name: <asp:TextBox ID="regUsername" runat="server" /></div>
                    <div class="login_field">Password: <asp:TextBox ID="regPassword" TextMode="Password" runat="server" /></div>
                    <div class="login_field">Confirm password: <asp:TextBox ID="regConfirmPassword" TextMode="Password" runat="server" /></div>
                    <div class="login_registration"><a href="login.aspx">Back to login</a></div>                    
                    <div class="login_message"><span id="regMessage" runat="Server" /></div>            
                    <div class="login_button"><asp:Button ID="regButton" OnClick="Register_Click" Text="  Register  " runat="server" /></div>
                </form>                    
            </div>
            <%
            break;
        default:
            %>
            <div class="login_main">
                <form id="loginForm" runat="server">
                    <div class="login_title">Login</div>
                    <div class="login_field">Username: <asp:textbox id="username" runat="Server"/></div>
                    <div class="login_field">Password: <asp:textbox id="password" textmode="Password" runat="Server"/></div>
                    <div class="login_registration"><a href="login.aspx?action=registration">Registration</a></div>
                    <div class="login_message"><span id="message" runat="Server" /></div>
                    <div class="login_button"><asp:Button ID="loginButton" OnClick="Login_Click" Text="  Login  " runat="server" /></div>
                </form>
            </div>                   
            <%
            break;
    }
    %>
</body>
</html>
