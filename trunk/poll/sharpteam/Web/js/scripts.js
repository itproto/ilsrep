function CheckIfSelectedChoice()
{
    var index;
    var choices = document.getElementsByName("choice");
    for (index = 0; index < choices.length; index++)
    {
        var curChoice = document.getElementById("choice_" + index);
        if (curChoice.checked == true)
        {
            return true;
        }
    }
    alert("Please, select a choice!");
    return false;
}