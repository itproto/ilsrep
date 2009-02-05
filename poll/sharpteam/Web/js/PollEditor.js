$(document).ready(function() {
    /*** hover for input fields ***/
    $(":text").focus(function() {
        $(this).addClass("hover");
    }).blur(function() {
        $(this).removeClass("hover");
    });

    $("button:not(:disabled)")
    .addClass("ui-state-default")
    .addClass("ui-corner-all")
    .hover(
        function(){
            $(this).addClass("ui-state-hover");
        },
        function(){
            $(this).removeClass("ui-state-hover");
        }
    );

    /*** Init Dialogs ***/
    $("#addPollDialog").dialog({ autoOpen: false, resizable: false, modal: true, overlay: { backgroundColor: '#000', opacity: 0.5 }, buttons: { "Add": addPoll, "Cancel": function() { $(this).dialog("close"); } } });
    $("#editPollDialog").dialog({ autoOpen: false, resizable: false, modal: true, overlay: { backgroundColor: '#000', opacity: 0.5 }, buttons: { "Edit": editPoll, "Cancel": function() { $(this).dialog("close"); } } });
    $("#addChoiceDialog").dialog({ autoOpen: false, resizable: false, modal: true, overlay: { backgroundColor: '#000', opacity: 0.5 }, buttons: { "Add": addChoice, "Cancel": function() { $(this).dialog("close"); } } });
    $("#editChoiceDialog").dialog({ autoOpen: false, resizable: false, modal: true, overlay: { backgroundColor: '#000', opacity: 0.5 }, buttons: { "Edit": editChoice, "Cancel": function() { $(this).dialog("close"); } } });
    $("#messageDialog").dialog({ autoOpen: false, resizable: false, modal: true, overlay: { backgroundColor: '#000', opacity: 0.5 }, buttons: { "OK": function() { $(this).dialog("close"); } } });
    $("#confirmDialog").dialog({ autoOpen: false, resizable: false, modal: true, overlay: { backgroundColor: '#000', opacity: 0.5 }, buttons: { "Yes": function() {  }, "No": function() {  } } });
    
    /*** constant links ***/		
    $("#survey_reset").click(function() { document.location = 'PollEditor.aspx?action=edit&id='+currentSurveyID+'&reset=1' });
    $("#add_poll").click(function() { $("#addPollDialog").dialog("open"); $("#addPollDialog :input").val(""); return false; });

    /*** get the polls ***/
    getPolls();
});

function getPolls()
{
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "PollEditor.aspx/GetPolls",
        data: "{ 'surveyID': " + currentSurveyID + " }",
        dataType: "json",
        beforeSend: function()
        {
            $("#survey_loading").show();
            $("#survey_tree").hide();        
        },
        success: function(data)
        {
            $("#survey_polls").setTemplateURL('js/jtemplates/polls.tpl', null, { filter_data: false });
            $('#survey_polls').processTemplate(data.d);
            $("#survey_tree").treeview();
            $("#survey_loading").hide();
            $("#survey_tree").show();
            $("#survey_tree").removeClass("hidden");

            // poll functions
            $(".edit_poll").click( function() {
                var pollObj = $(this).parents(".poll");
                var pollData = eval("(" + pollObj.attr("data") + ")");
                
                $("#editPollDialog").dialog("open");
                $("#editPollDialog :input[name=poll_id]").val(pollData.id);
                $("#editPollDialog :input[name=poll_name]").val(pollData.name);
                $("#editPollDialog :input[name=poll_desc]").val(pollData.description);
                $($("#editPollDialog :input[name=poll_custom]")[0].options).each(function(option){
                    if ($(this).attr("value") == pollData.custom)
                    {
                        $(this).parent()[0].selectedIndex = option;
                        return false;
                    }
                });
                
                return false;
            });
            $(".delete_poll").click(function() {
                var pollObj = $(this).parents(".poll");
                var pollData = eval("(" + pollObj.attr("data") + ")");
                
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "PollEditor.aspx/RemovePoll",
                    data: "{ 'surveyID': " + currentSurveyID + ", pollID: " + pollData.id + " }",
                    dataType: "json",
                    success: function(data)
                    {
                        if (data.d == true)
                        {
                            pollObj.remove();
                        }
                        else
                        {
                            $("#messageDialog").html("Poll you wanted to delete not found!");
                            $("#messageDialog").dialog("open");
                        }
                    }
                });
                
                return false;
            });
            $(".add_choice").click(function() {
                var pollObj = $(this).parents(".poll");
                var pollData = eval("(" + pollObj.attr("data") + ")");
            
                $("#addChoiceDialog").dialog("open");
                $("#addChoiceDialog :input").val("");
                $("#addChoiceDialog :input[name=poll_id]").val(pollData.id);
                return false;
            });
            $(".edit_choice").click(function() {
                var pollObj = $(this).parents(".poll");
                var pollData = eval("(" + pollObj.attr("data") + ")");
                
                var choiceObj = $(this).parents(".choice");
                var choiceID = choiceObj.attr("data");
                var choice = choiceObj.children(".file").html();
            
                $("#editChoiceDialog").dialog("open");
                $("#editChoiceDialog :input[name=choice]").val(choice);
                $("#editChoiceDialog :input[name=poll_id]").val(pollData.id);
                $("#editChoiceDialog :input[name=choice_id]").val(choiceID);
                
                return false;
            });
            $(".delete_choice").click(function() {
                var pollObj = $(this).parents(".poll");
                var pollData = eval("(" + pollObj.attr("data") + ")");
                
                var choiceObj = $(this).parents(".choice");
                var choiceID = choiceObj.attr("data");
                
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "PollEditor.aspx/RemoveChoice",
                    data: "{ 'surveyID': " + currentSurveyID + ", pollID: " + pollData.id + ", choiceID: " + choiceID + " }",
                    dataType: "json",
                    success: function(data)
                    {
                        if (data.d == true)
                        {
                            choiceObj.remove();
                        }
                        else
                        {
                            $("#messageDialog").html("Choice you wanted to delete not found!");
                            $("#messageDialog").dialog("open");
                        }
                    }
                });
                
                return false;
            });
        }
    });
}

function addPoll()
{
    var fields = {};
    $("#addPollDialog :input").each(function()
    {
        fields[$(this).attr("name")] = $(this).val();
    });
    fields["surveyID"] = currentSurveyID;    
    
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "PollEditor.aspx/AddPoll",
        data: '{"arguments": ' + JSON.stringify(fields) + '}',
        dataType: "json",
        success: function(data)
        {
            if (data.d == true)
            {
                $("#addPollDialog").dialog("close");
                getPolls();
            }
            else
            {
                $("#messageDialog").html("Fill in all fields!");
                $("#messageDialog").dialog("open");
            }
        }
    });
}

function editPoll()
{    
    var fields = {};
    $("#editPollDialog :input").each(function()
    {
        fields[$(this).attr("name")] = $(this).val();
    });
    fields["surveyID"] = currentSurveyID; 
    strFields = JSON.stringify(fields);   
    
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "PollEditor.aspx/EditPoll",
        data: '{"arguments": ' + strFields + '}',
        dataType: "json",
        success: function(data)
        {
            if (data.d == true)
            {
                $("#editPollDialog").dialog("close");
                var currentPoll = $("#survey_polls #poll_" + fields.poll_id)
                currentPoll.attr("data", strFields);
                currentPoll.children(".folder").html(fields.poll_name);
            }
            else
            {
                $("#messageDialog").html("Fill in all fields!");
                $("#messageDialog").dialog("open");
            }
        }
    });
}

function addChoice()
{
    var fields = {};
    $("#addChoiceDialog :input").each(function()
    {
        fields[$(this).attr("name")] = $(this).val();
    });
    fields["surveyID"] = currentSurveyID;
    
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "PollEditor.aspx/AddChoice",
        data: '{"arguments": ' + JSON.stringify(fields) + '}',
        dataType: "json",
        success: function(data)
        {
            if (data.d == true)
            {
                $("#addChoiceDialog").dialog("close");
                getPolls();
            }
            else
            {
                $("#messageDialog").html("Fill in all fields!");
                $("#messageDialog").dialog("open");
            }
        }
    });
}

function editChoice()
{
    var fields = {};
    $("#editChoiceDialog :input").each(function()
    {
        fields[$(this).attr("name")] = $(this).val();
    });
    fields["surveyID"] = currentSurveyID;
    
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "PollEditor.aspx/EditChoice",
        data: '{"arguments": ' + JSON.stringify(fields) + '}',
        dataType: "json",
        success: function(data)
        {
            if (data.d == true)
            {
                $("#editChoiceDialog").dialog("close");
                var currentPoll = $("#survey_polls #choice_" + fields.choice_id)
                currentPoll.children(".file").html(fields.choice);
            }
            else
            {
                $("#messageDialog").html("Fill in all fields!");
                $("#messageDialog").dialog("open");
            }
        }
    });
}