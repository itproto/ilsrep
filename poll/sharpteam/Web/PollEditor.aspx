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
    <%
    if (ShowPage == "editpollsession")
    {
    %>
        $(document).ready(function() 
            {
                $("#pollsession_tree").treeview();
                $("#addPollDialog").dialog({ autoOpen: false, resizable: false, modal: true, buttons: { "Add": addPoll, "Cancel": function() { $(this).dialog("close"); } } });
                $("#link_add_poll").click( function() { $("#addPollDialog").dialog("open"); return false; } );
                $("#pollsession_reset").click( function() { document.location='PollEditor.aspx?action=show&what=editpollsession&id=<%= selectedPollsession.Id %>&reset=1' });
                $(".links_delete_poll").click( function () {
                    var id = this.id.replace("link_delete_poll_", "");
                    $("#poll_"+id).remove();
                    $.post("PollEditor.aspx?action=delete&what=poll&pollsession_id=<%=selectedPollsession.Id%>&poll_id=" + id);
                });
            }
        );

        function addPoll(dialog) 
        {
            var fields = $("#addPollDialog :input");
            $.post("PollEditor.aspx?action=add&what=poll&pollsession_id=<%=selectedPollsession.Id%>", fields, addPollCallback, "json");
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
            var branches = $("<li id='poll_"+data.id+"'><span class='folder'>" + $(fields[0]).val() + "</span> <div><a href='#'><img src='js/treeview/images/page_white_add.png' /></a><a href='#'><img src='js/treeview/images/page_white_edit.png' /></a><a href='#' id='link_delete_poll_"+data.id+"' class='links_delete_poll'><img src='js/treeview/images/page_white_delete.png' /></a></div><ul></ul></li>").appendTo("#pollsession_tree>li>ul");
            $("#link_delete_poll_" + data.id).click( function() {
                var id = this.id.replace("link_delete_poll_", "");
                $("#poll_"+id).remove();
                $.post("PollEditor.aspx?action=delete&what=poll&pollsession_id=<%=selectedPollsession.Id%>&poll_id=" + id);
            });
            $("#pollsession_tree").treeview(
                {
                    add: branches
                }
            );
        }
    <%
    }
                    
    %>
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
                        <a href="PollEditor.aspx">Poll editor |</a>
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
                    <h3>Edit Pollsession:</h3>
                    <ul>
                    <%
                        int index = 1;
                        foreach (Ilsrep.PollApplication.Communication.Item pollSession in pollSessionsList)
                        {
                            %>
                                <li><a href="?action=show&what=editpollsession&id=<%=pollSession.id%>"><%=index%>. <%=pollSession.name%></a></li>
                            <%
                            ++index;
                        }
                    %>
                        <li><a href="?action=show&what=editpollsession&id=0">Add New</a></li>
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
                        <form action="PollEditor.aspx?action=edit&amp;what=pollsession&id=<%= selectedPollsession.Id %>" method="post">
                            Pollsession Name: <input type="text" name="pollsession_name" value="<%= selectedPollsession.Name %>" class="text" /><br />
                            
                            <ul id="pollsession_tree" class="filetree treeview-famfamfam">
                                <li>
                                    <span class="folder">Polls</span> 
                                    <div>
                                        <a href='#' id='link_add_poll'>
                                            <img alt="Add Poll" src='js/treeview/images/page_white_add.png' />
                                        </a>
                                    </div>
                                    <ul>
                                        <%
                                           foreach(Ilsrep.PollApplication.Model.Poll poll in selectedPollsession.Polls)
                                           {
                                               %>
                                               <li id='poll_<%=poll.Id %>'><span class='folder'><%=poll.Name%></span>
                                                   <div>
                                                       <a href='#'><img alt="Add" src='js/treeview/images/page_white_add.png' /></a> 
                                                       <a href='#'><img alt="Edit" src='js/treeview/images/page_white_edit.png' /></a> 
                                                       <a href='#' id='link_delete_poll_<%=poll.Id %>' class='links_delete_poll'><img alt="Delete" src='js/treeview/images/page_white_delete.png' /></a>
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
                            <input type="button" id="pollsession_reset" value="Reset" class="button" />
                        </form>
                        
                        <div id="addPollDialog" title="Add Poll">
                            Poll Name: <input type="text" name="poll_name" class="text" /><br />
                            Poll Description:<br />
                            <textarea name="poll_desc" rows="3" cols="30" class="text"></textarea>
                        </div>
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
