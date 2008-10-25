<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head>
    <title>PollClientASP</title>
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
                        <a href="?action=start_poll" class="button">Start poll |</a>
                    </li>
                    <li>
                        <a href="PollEditor.aspx" class="button">Poll editor</a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="centralBlock">
            <div class="leftBlock">
                <div class="leftMenu">
                    <ul>
                        <%
                            if (Request["action"] == "showpollsession")
                            {
                        
                        %>
                        <li><h3><%=pollSession.Name%>:</h3></li>
                        <%
                                int index = 1;
                                foreach (Ilsrep.PollApplication.Model.Poll curPoll in pollSession.Polls)
                                {
                                    if (Convert.ToInt32(Session["pollIndex"]) == index - 1)
                                        Response.Write("<li><b>" + index + ". " + curPoll.Name + "</b></li>");
                                    else
                                        Response.Write("<li>" + index + ". " + curPoll.Name + "</li>");
                                    index++;
                                }
                            }
                            else
                            {
                                foreach (Ilsrep.PollApplication.Communication.Item curItem in pollSessionsList)
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
                    <form class="choices" method="post" action="Default.aspx?action=showpollsession&do=submitpoll">
                    <h3><%=pollSession.Polls[Convert.ToInt32(Session["pollIndex"])].Description%></h3>
                    <%
                        int index = 0;
                        foreach (Ilsrep.PollApplication.Model.Choice curChoice in pollSession.Polls[Convert.ToInt32(Session["pollIndex"])].Choices)
                        {
                    %>
                            <label for="choice_<%=index%>"><input type="radio" onfocus="this.blur();" name="choice" id="choice_<%=index%>" value="<%=index%>" /><%=curChoice.choice%></label><br />
                    <%  
                            index++;
                        }
                    %>
                        <input type="submit" class="submitButton" value="Continue" onfocus="this.blur();" onclick="return CheckIfSelectedChoice();" />
                    </form>
                <%
                    }
                    else if (Request["action"] == "showresults")
                    {
                %>
                <div class='inner'>
                    <h3>Here is your PollSession results:<br /></h3>
                <%
                        float correctAnswers = 0;
                        int index = 0;
                        foreach (Ilsrep.PollApplication.Model.PollResult curResult in resultsList.results)
                        {
                            index++;                            
                            Response.Write(index + ". " + pollSession.Polls[curResult.questionId].Name + ": " + pollSession.Polls[curResult.questionId].Choices[curResult.answerId].choice + "<br />");
                            if (pollSession.TestMode)
                            {
                                if (pollSession.Polls[curResult.questionId].Choices[curResult.answerId].Id == pollSession.Polls[curResult.questionId].CorrectChoiceID)
                                    correctAnswers++;
                            }
                        }

                        if (pollSession.TestMode)
                        {
                            double userScore = correctAnswers / pollSession.Polls.Count;
                            Response.Write("<br />Your score: " + Convert.ToInt32(userScore * 100) + "%");

                            if (userScore >= pollSession.MinScore)
                            {
                                Response.Write("<br />Congratulations!!! You PASSED the test");
                            }
                            else
                            {
                                Response.Write("<br />Sorry, try again... you FAILED");
                            }
                        }
                %>
                </div>
                <%
                    }
                    else if (Request["action"] == "start_poll")
                    {
                        Response.Write("<div class='inner'><h3>Please, select PollSession</h3></div>");
                    }
                %>
            </div>
        </div>
        <div class="footer">Copyright &copy; Sharpteam 2008</div>
    </div>
</body>
</html>
