<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Statistics.aspx.cs" Inherits="Statistics" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head>
    <title>Statistics</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <link href="css/style.css" type="text/css" rel="stylesheet" />
    <script src="js/scripts.js" type="text/javascript"></script>
</head>
<body class="home">
    <div class="main">
        <div class="header">
            <div class="logo"></div>
            <div class="mainMenu">
                <ul>
                    <li>
                        <a onfocus="this.blur()" href="Default.aspx?action=startpoll" class="button">Start poll |</a>
                    </li>
                    <li>
                        <a onfocus="this.blur()" href="PollEditor.aspx" class="button">Poll editor |</a>
                    </li>
                    <li>
                        <a onfocus="this.blur()" href="Statistics.aspx" class="button">Statistics |</a>
                    </li>                    
                    <li>
                        <a onfocus="this.blur()" href="login.aspx?action=logout">Logout(<%=User.Identity.Name%>)</a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="centralBlock">
            <div class="leftBlock">
                <div class="leftMenu">
                    <asp:Panel ID="leftMenuPanel" runat="server"></asp:Panel>
                </div>
            </div>
            <div class="content">
                <asp:Panel ID="contentPanel" runat="server"></asp:Panel>
            </div>
            <div class="footer">Copyright &copy; Sharpteam 2008</div>
        </div>
    </div>
</body>
</html>

