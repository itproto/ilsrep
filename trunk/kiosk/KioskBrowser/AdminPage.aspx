<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminPage.aspx.cs" Inherits="KioskBrowser.AdminPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Admin Page</title>
    <link href="css/style.css" type="text/css" rel="Stylesheet" />
    <script src="js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="js/adminscript.js" type="text/javascript"></script>
</head>
<body>
    <form id="mainForm" runat="server">
    <div>
        <table class="admin_main_table" cellpadding="0" cellspacing="0">
            <tr>
                <td colspan="2" class="admin_main_title">Kiosk Admin</td>
            </tr>
            <tr>
                <td class="admin_left_menu">
                    <ul>
                        <li><asp:Button ID="credentialsButton" runat="server" CssClass="admin_button" Text="Credentials" /></li>
                        <li><asp:Button ID="programsButton" runat="server" CssClass="admin_button" Text="Programs" /></li>
                        <li><asp:Button ID="logoutButton" runat="server" CssClass="admin_button_logout" Text="LogOut" OnClick="LogOutButtonClick" /></li>
                    </ul>
                </td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
