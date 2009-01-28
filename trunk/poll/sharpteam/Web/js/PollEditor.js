$(document).ready(function() {
    $(document).ready(addHover);

    $("#addPollDialog").dialog({ autoOpen: false, resizable: false, modal: true, buttons: { "Add": addPoll, "Cancel": function() { $(this).dialog("close"); } } });
    $("#editPollDialog").dialog({ autoOpen: false, resizable: false, modal: true, buttons: { "Edit": editPoll, "Cancel": function() { $(this).dialog("close"); } } });
    $("#addChoiceDialog").dialog({ autoOpen: false, resizable: false, modal: true, buttons: { "Add": addChoice, "Cancel": function() { $(this).dialog("close"); } } });
    $("#editChoiceDialog").dialog({ autoOpen: false, resizable: false, modal: true, buttons: { "Edit": editChoice, "Cancel": function() { $(this).dialog("close"); } } });
    
    $("#survey_reset").click(function() { document.location = 'PollEditor.aspx?action=edit&id='+currentSurveyID+'&reset=1' });
    $("#add_poll").click(function() { $("#addPollDialog").dialog("open"); return false; });

    $.ajax({
    type: "POST",
    contentType: "application/json; charset=utf-8",
    url: "PollEditor.aspx/GetPolls",
    data: "{ 'surveyID': " + currentSurveyID + " }",
    dataType: "json",
    success: function(data)
    {
        $("#survey_polls").setTemplateURL('js/jtemplates/polls.tpl', null, { filter_data: false});
        $('#survey_polls').processTemplate(data.d);
        $("#survey_tree").treeview();
        $("#survey_loading").hide();
        $("#survey_tree").removeClass("hidden");
        
        // poll functions
        $(".edit_poll").click( function() {
            var pollObj = $(this).parents(".poll");
            var id = pollObj.attr("data");
            
            $("#editPollDialog").dialog("open");
            $("#editPollDialog :input[name=poll_id]").val(id);
            $("#editPollDialog :input[name=poll_name]").val(pollObj.children("span.folder").text());
            return false;
        });
        $(".delete_poll").click(function() {
            var pollObj = $(this).parents(".poll");
            var id = pollObj.attr("data");
            
            $("#poll_" + id).remove();
            //$.get("PollEditor.aspx?action=delete&what=poll&survey_id="+currentSurveyID+"&poll_id=" + id);
            return false;
        });
    }
    });

});
    

/*    

    

    
    $(".choice_img:not(.correct_choice)")
            .bind('click', function() { var ids = this.id.replace("correct_choice_", "").split("_"); selectCorrectChoice(ids[0], ids[1]); $(this).unbind().addClass('correct_choice'); })
            .bind("mouseenter mouseleave", function(e) {
                if ($(this).attr('src').search("_off") != -1) {
                    $(this).attr('src', $(this).attr('src').replace("_off", "_on"));
                }
                else {
                    $(this).attr('src', $(this).attr('src').replace("_on", "_off"));
                }
            });
*/


/*
 *	
 */
function refreshChoiceLinks() {
    /* choice functions */
    $(".links_add_choice").click(function() {
        var id = this.id.replace("link_add_choice_", "");
        $("#addChoiceDialog").dialog("open");
        $("#addChoiceDialog :input[name=poll_id]").val(id);
        return false;
    });
    $(".links_edit_choice").click(function() {
        var ids = this.id.replace("link_edit_choice_", "").split("_");
        $("#editChoiceDialog").dialog("open");
        $("#editChoiceDialog :input[name=poll_id]").val(ids[0]);
        $("#editChoiceDialog :input[name=choice_id]").val(ids[1]);
        $("#editChoiceDialog :input[name=choice]").val($("#choice_" + ids[1]).find("span").text());
        return false;
    });
    $(".links_delete_choice").click(function() {
        var ids = this.id.replace("link_delete_choice_", "").split("_");
        $.get("PollEditor.aspx?action=delete&what=choice&survey_id=<%=selectedSurvey.Id%>&poll_id=" + ids[0] + "&choice_id=" + ids[1], [], function(data) { if (data.response == 1) { $("#choice_" + data.id).remove(); } else { alert(data.error); } }, "json");
        return false;
    });
}


function addPoll() {
    var fields = $("#addPollDialog :input");
    $.post("PollEditor.aspx?action=add&what=poll&survey_id=<%=selectedSurvey.Id%>", fields, addPollCallback, "json");
    $(this).dialog("close");
}

function addPollCallback(data, textStatus) {
    if (data.response == -1) {
        alert(data.error);
        return false;
    }

    //var fields = $("#addPollDialog :input");
    var branches = $("<li></li>")
                .attr("id", "poll_" + data.id)
                .html(
                    $("<span></span>")
                        .addClass("folder")
                        .html(data.poll_name)
                )
                .append(
                    $("<div></div>")
                );


    var branches = $("<li id='poll_" + data.id + "'><span class='folder'>" + data.poll_name + "</span> <div><a href='#' id='link_add_choice_" + data.id + "' class='links_add_choice'><img src='js/treeview/images/page_white_add.png' /></a> <a href='#' id='link_edit_poll_" + data.id + "' class='links_edit_poll'><img src='js/treeview/images/page_white_edit.png' /></a> <a href='#' id='link_delete_poll_" + data.id + "' class='links_delete_poll'><img src='js/treeview/images/page_white_delete.png' /></a></div><ul></ul></li>").appendTo("#survey_tree>li>ul");




    $("#link_add_choice_" + data.id).click(function() {
        var id = this.id.replace("link_add_choice_", "");
        $("#addChoiceDialog").dialog("open");
        $("#addChoiceDialog :input[name=poll_id]").val(id);
        return false;
    });
    $("#link_delete_poll_" + data.id).click(function() {
        var id = this.id.replace("link_delete_poll_", "");
        $("#poll_" + id).remove();
        $.post("PollEditor.aspx?action=delete&what=poll&survey_id=<%=selectedSurvey.Id%>&poll_id=" + id);
    });
    $("#survey_tree").treeview(
                {
                    add: branches
                }
            );
}

function editPoll() {


}

function addChoice() {
    var fields = $("#addChoiceDialog :input");
    $.post("PollEditor.aspx?action=add&what=choice&survey_id=<%=selectedSurvey.Id%>", fields, addChoiceCallback, "json");
    $(this).dialog("close");
}

function addChoiceCallback(data, textStatus) {
    if (data.response == -1) {
        alert(data.error);
        return false;
    }

    if ($("#poll_" + data.poll_id + " .correct_choice").length == 0)
        image = "<img src='images/tick_on.png' alt='Correct Choice' id='correct_choice_" + data.poll_id + "_" + data.id + "' class='choice_img correct_choice' />";
    else
        image = "<img src='images/tick_off.png' alt='Correct Choice' id='correct_choice_" + data.poll_id + "_" + data.id + "' class='choice_img' />";

    var branches = $("<li id='choice_" + data.id + "'><span class='file'>" + data.choice + "</span> <div>" + image + " <a href='#' id='link_edit_choice_" + data.poll_id + "_" + data.id + "' class='links_edit_choice'><img alt='Edit' src='js/treeview/images/page_white_edit.png' /></a> <a href='#' id='link_delete_choice_" + data.poll_id + "_" + data.id + "' class='links_delete_choice'><img alt='Delete' src='js/treeview/images/page_white_delete.png' /></a></div></li>").appendTo("#survey_tree>li>ul #poll_" + data.poll_id + ">ul");
    $("#link_edit_choice_" + data.poll_id + "_" + data.id).click(function() {
        var ids = this.id.replace("link_edit_choice_", "").split("_");
        $("#editChoiceDialog").dialog("open");
        $("#editChoiceDialog :input[name=poll_id]").val(ids[0]);
        $("#editChoiceDialog :input[name=choice_id]").val(ids[1]);
        $("#editChoiceDialog :input[name=choice]").val($("#choice_" + ids[1]).find("span").text());
        return false;
    });
    $("#link_delete_choice_" + data.poll_id + "_" + data.id).click(function() {
        var ids = this.id.replace("link_delete_choice_", "").split("_");
        $.get("PollEditor.aspx?action=delete&what=choice&survey_id=<%=selectedSurvey.Id%>&poll_id=" + ids[0] + "&choice_id=" + ids[1], [], function(data) { if (data.response == 1) { $("#choice_" + data.id).remove(); } else { alert(data.error); } }, "json");
        return false;
    });
    $(".choice_img:not(.correct_choice)")
            .bind('click', function() { var ids = this.id.replace("correct_choice_", "").split("_"); selectCorrectChoice(ids[0], ids[1]); $(this).unbind().addClass('correct_choice'); })
            .bind("mouseenter mouseleave", function(e) {
                if ($(this).attr('src').search("_off") != -1) {
                    $(this).attr('src', $(this).attr('src').replace("_off", "_on"));
                }
                else {
                    $(this).attr('src', $(this).attr('src').replace("_on", "_off"));
                }
            });
    $("#survey_tree").treeview(
                {
                    add: branches
                }
            );
}

function editChoice() {
    var fields = $("#editChoiceDialog :input");
    $.post("PollEditor.aspx?action=edit&what=choice&survey_id=<%=selectedSurvey.Id%>", fields, editChoiceCallback, "json");
    $(this).dialog("close");
}

function editChoiceCallback(data, textStatus) {
    if (data.response == -1) {
        alert(data.error);
        return false;
    }

    $("#choice_" + data.id + " span").text(data.choice);
}

function selectCorrectChoice(poll_id, choice_id) {
    if ($("#poll_" + poll_id + " .correct_choice")[0].id.replace('correct_choice_', '') != choice_id) {
        $.get("PollEditor.aspx?action=correct&survey_id=<%=selectedSurvey.Id%>&poll_id=" + poll_id + "&choice_id=" + choice_id);

        $('#poll_' + poll_id + ' .correct_choice')
                    .attr('src', $('#poll_' + poll_id + ' .correct_choice').attr('src').replace("_on", "_off"))
                    .removeClass('correct_choice')
                    .bind('click', function() { var ids = this.id.replace("correct_choice_", "").split("_"); selectCorrectChoice(ids[0], ids[1]); $(this).unbind().addClass('correct_choice'); })
                    .bind("mouseenter mouseleave", function(e) {
                        if ($(this).attr('src').search("_off") != -1) {
                            $(this).attr('src', $(this).attr('src').replace("_off", "_on"));
                        }
                        else {
                            $(this).attr('src', $(this).attr('src').replace("_on", "_off"));
                        }
                    });
    }
}