$(document).ready(function() {
    $("input").focus(function() {
        this.blur();
    });
});

function addDigit(obj)
{
    document.getElementById('passTextBox').value += obj.value;
    return false;
}

function removeDigit()
{
    if (document.getElementById('passTextBox').value.length > 0)
        document.getElementById('passTextBox').value = document.getElementById('passTextBox').value.slice(0, -1);
    return false;
}