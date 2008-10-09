<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head runat="server">
    <title>PollClientASP</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <link href="css/style.css" type="text/css" rel="stylesheet" />
    <!--script src="js/scripts.js" type="text/javascript" /-->
</head>
<body class="home">
    <div class="main">
        <div class="header">
            <div class="logo"></div>
            <div class="mainMenu">
                <ul>
                    <li>
                        <a href="?action=start_poll" onfocus="this.blur()">Start poll</a> |
                    </li>
                    <li>
                        <a href="#" onfocus="this.blur()">Poll editor</a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="centralBlock">
            <div class="leftBlock">
                <div class="leftMenu">
                    <ul>
                        <%
                            if ( Request["action"] == "showpollsession" )
                            {
                                Response.Write("<h2>"+pollSession.name + ":</h2>\n");
                                int index = 1;
                                foreach ( Ilsrep.PollApplication.Model.Poll poll in pollSession.polls )
                                {
                                    if ( Convert.ToInt32(Session["pollIndex"]) == index-1 )
                                        Response.Write( "<li><b>"+index+". "+poll.name+"</b></li>" );
                                    else
                                        Response.Write("<li>"+index+". "+poll.name+"</li>");
                                    ++index;
                                }
                            }
                            else
                            {
                                foreach ( Ilsrep.PollApplication.Communication.Item curItem in pollSessionsList )
                                {
                        %>
                                <li>
                                    <a href="Default.aspx?action=showpollsession&id=<%=curItem.id%>" onfocus="this.blur()"><%=curItem.name%></a>
                                </li>
                        <%    
                                }
                            }
                        %>
                    </ul>
                </div>
            </div>
            <div class="content">
                <%
                if (Request["action"] == "showpollsession")
                {
                %>
                    <h3><%=pollSession.name%></h3>
                    <h3><%=pollSession.polls[Convert.ToInt32( Session["pollIndex"] )].name%></h3>
                    <div class="error"><%=errorMessage %></div>
                    <form class="choices" method="post" action="Default.aspx?action=showpollsession&do=submitpoll">
                    <input type="hidden" name="currentPoll" value="<%=Session["pollIndex"] %>" />
                    <%
                    foreach (Ilsrep.PollApplication.Model.Choice curChoice in pollSession.polls[Convert.ToInt32(Session["pollIndex"])].choices)
                    {
                    %>
                        <label for="choice_<%=curChoice.id %>"><input type="radio" name="choice" id="choice_<%=curChoice.id %>" value="<%=curChoice.id%>" /><%=curChoice.choice%></label><br />
                    <%  
                    }
                    %>
                        <input type="submit" value="Continue" />
                    </form>
                <%
                }
                else if (Request["action"] == "results")
                {
                %>   
                Results Page
                <%
                }
                %>
            </div>
        </div>
        <div class="footer">Copyright &copy; Sharpteam 2008</div>
    </div>
</body>
</html>
