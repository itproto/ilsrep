{#template MAIN}
{#foreach $T as poll}
{#param name=correctChoice value=$T.poll.CorrectChoiceID}
<li id="poll_{$T.poll.Id}" data="{#ldelim} id: '{$T.poll.Id}', name: '{$T.poll.Name}', description: '{$T.poll.Description}', custom: '{$T.poll.CustomChoiceEnabled}' {#rdelim}" class="poll">
    <span class="folder">{$T.poll.Name}</span>
    <div class="commands">
       <a href='#' class='add_choice'><img alt="Add" src='images/treeview/page_white_add.png' /></a> 
       <a href='#' class='edit_poll'><img alt="Edit" src='images/treeview/page_white_edit.png' /></a>        
       <a href='#' class='delete_poll'><img alt="Delete" src='images/treeview/page_white_delete.png' /></a>
    </div>
    
    {#include choices root=$T.poll.Choices}
</li>
{#/for}
{#/template MAIN}

{#template choices}
{#foreach $T as choice}
<ul>
    <li id="choice_{$T.choice.Id}" data="{$T.choice.Id}" class="choice {#if $T.choice.Id == $P.correctChoice}selected_choice{#/if}">
    <span class="file">{$T.choice.choice}</span>
    <div class="commands">
        <a href='#' class='edit_choice'><img alt="Edit" src='images/treeview/page_white_edit.png' /></a>
        <a href='#' class='delete_choice'><img alt="Delete" src='images/treeview/page_white_delete.png' /></a>
        <a href='#' class="correct_choice"><img src="images/tick_{#if $T.choice.Id == $P.correctChoice}on{#else}off{#/if}.png" /></a>
    </div>
    </li>
</ul>
{#/for}
{#/template choices}