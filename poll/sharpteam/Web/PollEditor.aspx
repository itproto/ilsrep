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
    if (ShowPage == "editsurvey")
    {
    %>
        $(document).ready(function() 
        {
            $("#survey_tree").treeview();
            $("#addPollDialog").dialog({ autoOpen: false, resizable: false, modal: true, buttons: { "Add": addPoll, "Cancel": function() { $(this).dialog("close"); } } });
            $("#addChoiceDialog").dialog({ autoOpen: false, resizable: false, modal: true, buttons: { "Add": addChoice, "Cancel": function() { $(this).dialog("close"); } } });
            $("#editChoiceDialog").dialog({ autoOpen: false, resizable: false, modal: true, buttons: { "edit": editChoice, "Cancel": function() { $(this).dialog("close"); } } });
            
            $("#survey_reset").click( function() { document.location='PollEditor.aspx?action=show&what=editsurvey&id=<%= selectedSurvey.Id %>&reset=1' });
            
            /* choice functions */
            $(".links_add_choice").click( function() {
                var id = this.id.replace("link_add_choice_", "");
                $("#addChoiceDialog").dialog("open");
                $("#addChoiceDialog :input[name=poll_id]").val(id);
                return false;
            } );
            $(".links_edit_choice").click( function() {
                var ids = this.id.replace("link_edit_choice_", "").split("_");
                $("#editChoiceDialog").dialog("open");
                $("#editChoiceDialog :input[name=poll_id]").val(ids[0]);
                $("#editChoiceDialog :input[name=choice_id]").val(ids[1]);
                $("#editChoiceDialog :input[name=choice]").val($("#choice_"+ids[1]).find("span").text());
                return false;
            } );
            $(".links_delete_choice").click( function () {
                var ids = this.id.replace("link_delete_choice_", "").split("_");
                $("#choice_"+ids[1]).remove();
                $.get("PollEditor.aspx?action=delete&what=choice&survey_id=<%=selectedSurvey.Id%>&poll_id="+ids[0]+"&choice_id=" + ids[1]);
                return false;
            });
            $(".choice_img:not(.correct_choice)")
            .bind('click', function() { var ids = this.id.replace("correct_choice_", "").split("_"); selectCorrectChoice(ids[0], ids[1]); $(this).unbind().addClass('correct_choice'); })
            .bind("mouseenter mouseleave", function(e){
                if ( $(this).attr('src').search("_off") != -1 )
                {
                    $(this).attr('src', $(this).attr('src').replace("_off", "_on"));
                }
                else
                {
                    $(this).attr('src', $(this).attr('src').replace("_on", "_off"));
                }
            });
            
            /* poll functions */
            $("#link_add_poll").click( function() { $("#addPollDialog").dialog("open"); return false; } );
            $(".links_delete_poll").click( function () {
                var id = this.id.replace("link_delete_poll_", "");
                $("#poll_"+id).remove();
                $.get("PollEditor.aspx?action=delete&what=poll&survey_id=<%=selectedSurvey.Id%>&poll_id=" + id);
                return false;
            });
        });

        function addPoll() 
        {
            var fields = $("#addPollDialog :input");
            $.post("PollEditor.aspx?action=add&what=poll&survey_id=<%=selectedSurvey.Id%>", fields, addPollCallback, "json");
            $(this).dialog("close");
        }

        function addPollCallback(data, textStatus)
        {
            if (data.response == -1)
            {
                alert(data.error);
                return false;
            }
        
            //var fields = $("#addPollDialog :input");
            var branches = $("<li id='poll_"+data.id+"'><span class='folder'>" + data.poll_name + "</span> <div><a href='#' id='link_add_choice_"+data.id+"' class='links_add_choice'><img src='js/treeview/images/page_white_add.png' /></a><a href='#' id='link_edit_poll_"+data.id+"' class='links_edit_poll'><img src='js/treeview/images/page_white_edit.png' /></a><a href='#' id='link_delete_poll_"+data.id+"' class='links_delete_poll'><img src='js/treeview/images/page_white_delete.png' /></a></div><ul></ul></li>").appendTo("#survey_tree>li>ul");
            $(".link_add_choice_" + data.id).click( function() {
                var id = this.id.replace("link_add_choice_", "");
                $("#addChoiceDialog").dialog("open");
                $("#addChoiceDialog :input[name=poll_id]").val(id);
                return false;
            } );
            $("#link_delete_poll_" + data.id).click( function() {
                var id = this.id.replace("link_delete_poll_", "");
                $("#poll_"+id).remove();
                $.post("PollEditor.aspx?action=delete&what=poll&survey_id=<%=selectedSurvey.Id%>&poll_id=" + id);
            });
            $("#survey_tree").treeview(
                {
                    add: branches
                }
            );
        }
        
        function addChoice()
        {
            var fields = $("#addChoiceDialog :input");
            $.post("PollEditor.aspx?action=add&what=choice&survey_id=<%=selectedSurvey.Id%>", fields, addChoiceCallback, "json");
            $(this).dialog("close");
        }
        
        function addChoiceCallback(data, textStatus)
        {
            if (data.response == -1)
            {
                alert(data.error);
                return false;
            }
            
            var branches = $("<li id='choice_"+data.id+"'><span class='file'>"+data.choice+"</span> <div><a href='#' id='link_edit_choice_"+data.poll_id+"_"+data.id+"' class='links_edit_choice'><img alt='Edit' src='js/treeview/images/page_white_edit.png' /></a> <a href='#' id='link_delete_choice_"+data.poll_id+"_"+data.id+"' class='links_delete_choice'><img alt='Delete' src='js/treeview/images/page_white_delete.png' /></a></div></li>").appendTo("#survey_tree>li>ul #poll_" + data.poll_id + ">ul");
            $("#link_edit_choice_"+data.poll_id+"_"+data.id).click( function() {
                var ids = this.id.replace("link_edit_choice_", "").split("_");
                $("#editChoiceDialog").dialog("open");
                $("#editChoiceDialog :input[name=poll_id]").val(ids[0]);
                $("#editChoiceDialog :input[name=choice_id]").val(ids[1]);
                $("#editChoiceDialog :input[name=choice]").val($("#choice_"+ids[1]).find("span").text());
                return false;
            } );
            $("#link_delete_choice_"+data.poll_id+"_"+data.id).click( function () {
                var ids = this.id.replace("link_delete_choice_", "").split("_");
                $("#choice_"+ids[1]).remove();
                $.get("PollEditor.aspx?action=delete&what=choice&survey_id=<%=selectedSurvey.Id%>&poll_id="+ids[0]+"&choice_id=" + ids[1]);
                return false;
            });
            $("#survey_tree").treeview(
                {
                    add: branches
                }
            );
        }
        
        function editChoice()
        {
            var fields = $("#editChoiceDialog :input");
            $.post("PollEditor.aspx?action=edit&what=choice&survey_id=<%=selectedSurvey.Id%>", fields, editChoiceCallback, "json");
            $(this).dialog("close");
        }
        
        function editChoiceCallback(data, textStatus)
        {
            if (data.response == -1)
            {
                alert(data.error);
                return false;
            }
            
            $("#choice_" + data.id + " span").text( data.choice );
        }
        
        function selectCorrectChoice(poll_id, choice_id)
        {
            if ( $("#poll_" + poll_id + " .correct_choice")[0].id.replace('correct_choice_', '') != choice_id )
            {
                $.get("PollEditor.aspx?action=correct&survey_id=<%=selectedSurvey.Id%>&poll_id=" + poll_id + "&choice_id=" + choice_id);
                
                $('#poll_' + poll_id + ' .correct_choice')
                    .attr('src', $('#poll_' + poll_id + ' .correct_choice').attr('src').replace("_on", "_off"))
                    .removeClass('correct_choice')
                    .bind('click', function() { var ids = this.id.replace("correct_choice_", "").split("_"); selectCorrectChoice(ids[0], ids[1]); $(this).unbind().addClass('correct_choice'); })
                    .bind("mouseenter mouseleave", function(e){
                        if ( $(this).attr('src').search("_off") != -1 )
                        {
                            $(this).attr('src', $(this).attr('src').replace("_off", "_on"));
                        }
                        else
                        {
                            $(this).attr('src', $(this).attr('src').replace("_on", "_off"));
                        }
                    });
            }
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
                        <a href="Default.aspx?action=start_poll">Start poll</a> |
                    </li>
                    <li>
                        <a href="PollEditor.aspx">Poll editor</a> |
                    </li>
                    <li>
                        <a href="login.aspx?action=logout">Logout</a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="centralBlock">
            <div class="leftBlock">
                <div class="leftMenu">
                    <h3>Edit Survey:</h3>
                    <ul>
                    <%
                        int index = 1;
                        foreach (Ilsrep.PollApplication.Communication.Item survey in surveysList)
                        {
                            %>
                                <li><a href="?action=show&what=editsurvey&id=<%=survey.id%>"><%=index%>. <%=survey.name%></a></li>
                            <%
                            ++index;
                        }
                    %>
                        <li><a href="?action=show&what=editsurvey&id=0">Add New</a></li>
                    </ul>
                </div>
            </div>
            <div class="content">
            <div class="inner">
                <%
                    if (ShowPage == "editsurvey")
                    {
                %>
                        <h3>Edit Survey</h3>
                        <div class="error"><%= Message %></div>
                        <form action="PollEditor.aspx?action=edit&amp;what=survey&id=<%= selectedSurvey.Id %>" method="post">
                            Survey Name: <input type="text" name="survey_name" value="<%= selectedSurvey.Name %>" class="text" /><br />
                            
                            <ul id="survey_tree" class="filetree treeview-famfamfam">
                                <li>
                                    <span class="folder">Polls</span> 
                                    <div>
                                        <a href='#' id='link_add_poll'>
                                            <img alt="Add Poll" src='js/treeview/images/page_white_add.png' />
                                        </a>
                                    </div>
                                    <ul>
                                        <%
                                           foreach(Ilsrep.PollApplication.Model.Poll poll in selectedSurvey.Polls)
                                           {
                                               %>
                                               <li id='poll_<%=poll.Id %>'><span class='folder'><%=poll.Name%></span>
                                                   <div>
                                                       <a href='#' id='link_add_choice_<%=poll.Id %>' class='links_add_choice'><img alt="Add" src='js/treeview/images/page_white_add.png' /></a> 
                                                       <a href='#' id='link_edit_poll_<%=poll.Id %>' class='links_edit_poll'><img alt="Edit" src='js/treeview/images/page_white_edit.png' /></a>        
                                                       <a href='#' id='link_delete_poll_<%=poll.Id %>' class='links_delete_poll'><img alt="Delete" src='js/treeview/images/page_white_delete.png' /></a>
                                                   </div>
                                                   <ul>
                                                   <%
                                                   foreach (Ilsrep.PollApplication.Model.Choice choice in poll.Choices)
                                                   {
                                                       %>
                                                       <li id='choice_<%=choice.Id %>'><span class='file'><%=choice.choice %></span>
                                                       <div>
                                                           <% if (poll.CorrectChoiceID == choice.Id) { %>
                                                           <img src="images/tick_on.png" alt="Correct Choice" id="correct_choice_<%= poll.Id %>_<%= choice.Id %>" class="correct_choice choice_img" />
                                                           <% } else { %>
                                                           <img src="images/tick_off.png" alt="Select Correct Choice" id="correct_choice_<%= poll.Id %>_<%= choice.Id %>" class="choice_img" />
                                                           <% } %>
                                                           <a href='#' id='link_edit_choice_<%=poll.Id %>_<%=choice.Id %>' class='links_edit_choice'><img alt="Edit" src='js/treeview/images/page_white_edit.png' /></a>
                                                           <a href='#' id='link_delete_choice_<%=poll.Id %>_<%=choice.Id %>' class='links_delete_choice'><img alt="Delete" src='js/treeview/images/page_white_delete.png' /></a>
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
                            
                            <input type="submit" value="<%= (selectedSurvey.Id < 0 ? "Add" : "Edit") %> Survey" class="button" />
                            <input type="button" id="survey_reset" value="Reset" class="button" />
                        </form>
                        
                        <div id="addPollDialog" title="Add Poll">
                            Poll Name: <input type="text" name="poll_name" class="text" /><br />
                            Poll Description:<br />
                            <textarea name="poll_desc" rows="3" cols="30" class="text"></textarea>
                            Enable Custom Choice: <input type="radio" name="poll_custom" value="true" /> Yes <input type="radio" name="poll_custom" value="false" /> No<br />
                        </div>
                        
                        <div id="addChoiceDialog" title="Add Choice">
                            <input type="hidden" name="poll_id" value="" />
                            Choice: <input type="text" name="choice" class="text" /><br /> 
                        </div>
                        
                        <div id="editChoiceDialog" title="Edit Choice">
                            <input type="hidden" name="poll_id" value="" />
                            <input type="hidden" name="choice_id" value="" />
                            Choice: <input type="text" name="choice" class="text" /><br /> 
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
