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
                        <a onfocus="this.blur()" href="Default.aspx?action=startpoll" class="button">Start poll |</a>
                    </li>
                    <li>
                        <a onfocus="this.blur()" href="PollEditor.aspx" class="button">Poll editor |</a>
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
                    <ul>
                        <%
                        int index;
                        if ((Request["action"] == "showsurvey") || (Request["action"] == "submitpoll"))
                        {
                            %>
                            <li><h3><%=survey.Name%>:</h3></li>
                            <%
                            index = 1;
                            foreach (Ilsrep.PollApplication.Model.Poll curPoll in survey.Polls)
                            {
                                if (Convert.ToInt32(Session["pollIndex"]) == index - 1)
                                {
                                    %>
                                    <li><b><%=index%>. <%=curPoll.Name%></b></li>
                                    <%
                                }
                                else
                                {
                                    %>
                                    <li><%=index%>. <%=curPoll.Name%></li>
                                    <%
                            }

                                index++;
                            }
                        }
                        else
                        {
                            %>
                            <h3>Select Survey:</h3>                                      
                            <%
                            index = 1;
                            foreach (Ilsrep.PollApplication.Communication.Item curItem in surveysList)
                            {
                                %>
                                <li>
                                    <a href="Default.aspx?action=showsurvey&id=<%=curItem.id%>" onfocus="this.blur()"><%=index%>. <%=curItem.name%></a>
                                </li>
                                <% 
                            index++;
                            }
                        }
                        %>
                    </ul>
                </div>
            </div>
            <div class="content">
                <%
                if ((Request["action"] == "showsurvey") || (Request["action"] == "submitpoll"))
                {
                    %>
                    <form class="choices" method="post" action="Default.aspx?action=submitpoll">
                    <h3><%=survey.Polls[Convert.ToInt32(Session["pollIndex"])].Description%></h3>
                    <%
                    index = 0;
                    foreach (Ilsrep.PollApplication.Model.Choice curChoice in survey.Polls[Convert.ToInt32(Session["pollIndex"])].Choices)
                    {
                        if (index == 0)
                        {
                            %>
                            <label for="choice_<%=index%>"><input checked="true" type="radio" onfocus="this.blur();" name="choice" id="Radio1" value="<%=index%>" /><%=curChoice.choice%></label><br />
                            <%  
                        }
                        else
                        {
                            %>
                            <label for="choice_<%=index%>"><input type="radio" onfocus="this.blur();" name="choice" id="choice_<%=index%>" value="<%=index%>" /><%=curChoice.choice%></label><br />
                            <%  
                        }
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
                    <h3>Here is your Survey results:<br /></h3>
                    <%
                    float correctAnswers = 0;
                    index = 0;
                    foreach (Ilsrep.PollApplication.Model.PollResult curResult in resultsList.results)
                    {
                        index++;  
                        %>                          
                        <%=index%>. <%=survey.Polls[curResult.questionId].Name%>: <%=survey.Polls[curResult.questionId].Choices[curResult.answerId].choice%><br />
                        <%
                        if (survey.TestMode)
                        {
                            if (survey.Polls[curResult.questionId].Choices[curResult.answerId].Id == survey.Polls[curResult.questionId].CorrectChoiceID)
                                correctAnswers++;
                        }
                    }

                    if (survey.TestMode)
                    {
                        double userScore = correctAnswers / survey.Polls.Count;
                        %>
                        <br />Your score: <%=Convert.ToInt32(userScore * 100)%>%
                        <%
                        if (userScore >= survey.MinScore)
                        {
                            %>
                            <br />Congratulations!!! You PASSED the test
                            <%
                        }
                        else
                        {
                            %>
                            <br /><br />Sorry, try again... you FAILED
                            <%
                        }
                    }
                }
                %>
                </div>
            </div>
            <div class="footer">Copyright &copy; Sharpteam 2008</div>
        </div>
    </div>
</body>
</html>
