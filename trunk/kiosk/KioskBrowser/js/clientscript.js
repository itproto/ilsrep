$(document).ready(function() {
    $("#mainFrame").load(function() {
        // How to get a current url of iframe content???
    });
    
    $(".menu_button").focus(function() {
        this.blur();
    });
    
    $("#logoutButton").focus(function() {
        this.blur();
    });
});