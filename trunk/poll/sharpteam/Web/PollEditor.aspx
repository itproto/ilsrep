<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="PollEditor.aspx.cs" Inherits="PollEditor" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head>
    <title>PollClientASP</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <link href="css/style.css" type="text/css" rel="stylesheet" />
    <link rel="stylesheet" href="js/treeview/jquery.treeview.css" type="text/css" media="screen" />
    <script src="js/jquery-1.2.6.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="js/jquery-ui-personalized-1.6rc2.min.js"></script>
    <script src="js/treeview/jquery.treeview.min.js" type="text/javascript"></script>
    <script src="js/scripts.js" type="text/javascript"></script>
    <script type="text/javascript">
    $(document).ready(addHover);
    </script>
</head>
<body class="home">
    <div class="main">
        <div class="header">
            <div class="logo"></div>
            <div class="mainMenu">
                <ul>
                    <li>
                        <a href="Default.aspx?action=start_poll">Start poll |</a>
                    </li>
                    <li>
                        <a href="PollEditor.aspx">Poll editor</a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="centralBlock">
            <div class="leftBlock">
                <div class="leftMenu">
                    <h3>Edit Pollsession:</h3>
                    <ul>
                    <%
                        int index = 1;
                        foreach (Ilsrep.PollApplication.Communication.Item pollSession in pollSessionsList)
                        {
                            %>
                                <li><a href="?do=show&what=editpollsession&id=<%=pollSession.id%>"><%=index%>. <%=pollSession.name%></a></li>
                            <%
                            ++index;
                        }
                    %>
                        <li><a href="?do=show&what=editpollsession&id=0">Add New</a></li>
                    </ul>
                </div>
            </div>
            <div class="content">
            <div class="inner">
                <%
                    if (ShowPage == "editpollsession")
                    {
                %>
                        <h3>Edit Pollsession</h3>
                        <div class="error"><%= Message %></div>
                        <form action="PollEditor.aspx?do=edit&amp;what=pollsession&id=<%= selectedPollsession.Id %>" method="post">
                            Pollsession Name: <input type="text" name="pollsession_name" value="<%= selectedPollsession.Name %>" class="text" /><br />
                            
                            <ul id="pollsession_tree" class="filetree treeview-famfamfam">
                                <li>
                                    <span class="folder">Polls</span> 
                                    <div>
                                        <a href='#' onclick='$("#addPollDialog").dialog("open"); return false;'>
                                            <img alt="Add Poll" src='js/treeview/images/page_white_add.png' />
                                        </a>
                                    </div>
                                    <ul>
                                        <%
                                           foreach(Ilsrep.PollApplication.Model.Poll poll in selectedPollsession.Polls)
                                           {
                                               %>
                                               <li><span class='folder'><%=poll.Name%></span>
                                                   <div>
                                                       <a href='#'><img alt="Add" src='js/treeview/images/page_white_add.png' /></a> 
                                                       <a href='#'><img alt="Edit" src='js/treeview/images/page_white_edit.png' /></a> 
                                                       <a href='#'><img alt="Delete" src='js/treeview/images/page_white_delete.png' /></a>
                                                   </div>
                                                   <ul>
                                                   <%
                                                   foreach (Ilsrep.PollApplication.Model.Choice choice in poll.Choices)
                                                   {
                                                       %>
                                                       <li><span class='file'><%=choice.choice%></span>
                                                       <div>
                                                           <a href='#'><img alt="Edit" src='js/treeview/images/page_white_edit.png' /></a>
                                                           <a href='#'><img alt="Delete" src='js/treeview/images/page_white_delete.png' /></a>
                                                       </div>
                                                       </li>
                                                       <%
                                                   }
                                                   %>
                                                   </ul>
                                               </li>
                                               <%
                                           }
                                        %>
                                    </ul>
                                </li>
                            </ul>
                            
                            <input type="submit" value="<%= (selectedPollsession.Id < 0 ? "Add" : "Edit") %> Pollsession" class="button" />
                            <input type="button" value="Reset" onclick="document.location='PollEditor.aspx?do=show&what=editpollsession&id=<%= selectedPollsession.Id %>&reset=1'" class="button" />
                        </form>
                        
                        <div id="addPollDialog" title="Add Poll">
                            Poll Name: <input type="text" name="poll_name" class="text" /><br />
                            Poll Description:<br />
                            <textarea name="poll_description" rows="3" cols="30" class="text"></textarea>
                        </div>
                        
                        <script type="text/javascript">
                            $(document).ready(function() 
                                {
                                    $("#pollsession_tree").treeview();
                                    $("#addPollDialog").dialog({ autoOpen: false, resizable: false, modal: true, buttons: { "Add": addPoll, "Cancel": function() { $(this).dialog("close"); } } });
                                    
                                }
                            );

                            function addPoll(dialog) 
                            {
                                var fields = $("#addPollDialog :input");
                                $.post("PollEditor.aspx?do=add&what=poll&pollsession_id=<%=selectedPollsession.Id%>", fields, addPollCallback);
                                $(this).dialog("close");
                            }

                            function addPollCallback(data, textStatus)
                            {
                                if (data.response == -1)
                                {
                                    alert(data.error);
                                    return false;
                                }
                            
                                var fields = $("#addPollDialog :input");
                                var branches = $("<li><span class='folder'>" + $(fields[0]).val() + "</span> <div><a onfocus='this.blur()' href='#'><img src='js/treeview/images/page_white_add.png' /></a><a onfocus='this.blur()' href='#'><img src='js/treeview/images/page_white_edit.png' /></a><a onfocus='this.blur()' href='#'><img src='js/treeview/images/page_white_delete.png' /></a></div></li>").appendTo("#pollsession_tree>li>ul");
                                $("#pollsession_tree>li>ul").treeview(
                                    {
                                        add: branches
                                    }
                                );
                            }
                        </script>
                <%
                    }
                %>
            </div>
            </div>
        </div>
        <div class="footer">Copyright &copy; Sharpteam 2008</div>
    </div>
</body>
</html>
