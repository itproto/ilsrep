<%@ Page Language="C#" AutoEventWireup="true" CodeFile="messages.aspx.cs" Inherits="messages" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head runat="server">
    <title>Message</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <link href="css/style.css" type="text/css" rel="Stylesheet" />
</head>
<body class="login_body">
    <form id="messageForm" runat="server">
        <div class="messages_main">
            <%
            switch (Request["type"])
            {
                case "deny_access":
                    %>
                    <div class="messages_title">Access denied</div>
                    <div class="messages_message">Sorry, access denied. You must be logged in as admin to view this page</div>
                    <div class="messages_return_button"><a href="<%=Request["redirect"]%>">Return</a></div>
                    <%
                    break;
                case "error404":
                    %>
                    <div class="messages_title">Error 404</div>
                    <div class="messages_message">Page not found</div>
                    <%
                    break;
                default:
                    %>
                    <div class="messages_title">Error</div>
                    <div class="messages_message">An error occured</div>
                    <%
                    break;
            }
            %>
        </div>
    </form>    
</body>
</html>
