<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientPage.aspx.cs" Inherits="KioskBrowser.ClientPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Client Page</title>
    <link href="css/style.css" type="text/css" rel="Stylesheet" />
    <script src="js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="js/clientscript.js" type="text/javascript"></script>
</head>
<body>
    <form id="mainForm" runat="server">
        <div class="menu_container">
            <table class="menu_table" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:DropDownList CssClass="programs_list" ID="programsList" runat="server"></asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button CssClass="run_button" ID="runButton" runat="server" Text="Run" OnClick="RunButtonClick" />
                    </td>
                    <td>
                        <asp:Button CssClass="logout_button" ID="logoutButton" runat="server" Text="LogOut" OnClick="LogoutButtonClick" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="address_string_container">
            <asp:TextBox CssClass="address_string" ID="addressString" runat="server" Text="http://www.google.com"></asp:TextBox>
        </div>
        <div class="main_frame_container">
            <iframe class="main_frame" id="mainFrame" runat="server"></iframe>
        </div>
    </form>
</body>
</html>
