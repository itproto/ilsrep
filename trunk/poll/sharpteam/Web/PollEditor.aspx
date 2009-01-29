<%@ Page Title="Poll Editor" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="PollEditor.aspx.cs" Inherits="_PollEditor" %>

<asp:Content ID="headPollEditor" ContentPlaceHolderID="head" Runat="Server">
    <link rel="stylesheet" href="css/jquery.treeview.css" type="text/css" media="screen" />
    <link rel="stylesheet" href="css/ui.all.css" type="text/css" media="screen" />
    <script src="js/jquery-1.3.1.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-personalized-1.6rc5.min.js" type="text/javascript"></script>
    <script src="js/jquery.treeview.min.js" type="text/javascript"></script>
    <script src="js/jquery-jtemplates.js" type="text/javascript"></script>
    <% if (selectedSurvey != null) { %>
    <script type='text/javascript'>
        var currentSurveyID = "<%= selectedSurvey.Id %>";
    </script>
    <script src='js/PollEditor.js' type='text/javascript'></script>
    <% } %>
</asp:Content>

<asp:Content ID="leftContentPollEditor" ContentPlaceHolderID="leftContent" Runat="Server">
    <asp:ListView runat="server" ID="surveyListMenu">
        <LayoutTemplate>
            <h3>Edit Survey:</h3>
            <ol>
                <li id="itemPlaceholder" runat="server" />
            </ol>
            <h3>Add Survey:</h3>
            <ul>
                <li><a href="?action=show&amp;what=editsurvey&amp;id=0">Add New</a></li>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <a href="?action=edit&amp;id=<%# Eval("id") %>"><%# Eval("name") %></a>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>

<asp:Content ID="mainContentPollEditor" ContentPlaceHolderID="mainContent" Runat="Server">
<% if (selectedSurvey != null) { %>
    <h3>Edit Survey</h3>
    <div class="error"><%= Message %></div>
    <form action="PollEditor.aspx?action=edit&amp;what=survey&amp;id=<%= selectedSurvey.Id %>" method="post">
        Survey Name: <input type="text" name="survey_name" value="<%= selectedSurvey.Name %>" class="text" /><br />

        <div id="survey_loading">
            <img src="images/ajax-loader.gif" alt="Loading" /> Loading survey...
        </div>
        
        <ul id="survey_tree" class="filetree treeview-famfamfam treeview hidden">
            <li>
                <span class="folder">Polls</span> 
                <div class="commands">
                    <a href='#' id='add_poll'>
                        <img alt="Add Poll" src='images/treeview/page_white_add.png' />
                    </a>
                </div>
                
                <ul id="survey_polls">
                    <li>Here goes polls...</li>
                </ul>
            </li>
        </ul>
        
        <button type="submit" name="save" class="ui-state-default ui-corner-all">Save Survey</button>
        <button id="survey_reset" class="ui-state-default ui-corner-all">Reset</button>
    </form>
    
    <div id="addPollDialog" title="Add Poll">
        Poll Name:<br />
        <input type="text" name="poll_name" class="text" /><br />
        Poll Description:<br />
        <textarea name="poll_desc" rows="3" cols="30" class="text"></textarea><br />
        <select name="poll_custom"><option value="false">No</option><option value="true">Yes</option></select>
        Enable Custom Choice: 
    </div>
    
    <div id="editPollDialog" title="Edit Poll">
        <input type="hidden" name="poll_id" value="" />
        Poll Name:<br />
        <input type="text" name="poll_name" class="text" /><br />
        Poll Description:<br />
        <textarea name="poll_desc" rows="3" cols="30" class="text"></textarea><br />
        <select name="poll_custom"><option value="false">No</option><option value="true">Yes</option></select>
        Enable Custom Choice:
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
<% } else { %>
Please select survey to edit from the left menu, or add new survey.
<% } %>
</asp:Content>