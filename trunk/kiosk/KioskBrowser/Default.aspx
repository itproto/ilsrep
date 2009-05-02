<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="KioskBrowser._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Kiosk Browser</title>
    <link href="css/style.css" type="text/css" rel="Stylesheet" />
    <script src="js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="js/script.js" type="text/javascript"></script>
</head>
<body>
    <form id="mainForm" runat="server">
        <div>
            <table class="login_table" cellpadding="0" cellspacing="0">
                <tr>
                    <td colspan="3" class="message_td">
                        <asp:Label CssClass="info_message" ID="messageLabel" runat="server">Please, type your login code</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:TextBox CssClass="pass_text_box" ID="passTextBox" runat="server" ReadOnly="true"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td><asp:Button CssClass="button" ID="button1" runat="server" Text="1" OnClientClick="return addDigit(this)" /></td>
                    <td><asp:Button CssClass="button" ID="button2" runat="server" Text="2" OnClientClick="return addDigit(this)" /></td>
                    <td><asp:Button CssClass="button" ID="button3" runat="server" Text="3" OnClientClick="return addDigit(this)" /></td>
                </tr>
                <tr>
                    <td><asp:Button CssClass="button" ID="button4" runat="server" Text="4" OnClientClick="return addDigit(this)" /></td>
                    <td><asp:Button CssClass="button" ID="button5" runat="server" Text="5" OnClientClick="return addDigit(this)" /></td>
                    <td><asp:Button CssClass="button" ID="button6" runat="server" Text="6" OnClientClick="return addDigit(this)" /></td>
                </tr>
                <tr>
                    <td><asp:Button CssClass="button" ID="button7" runat="server" Text="7" OnClientClick="return addDigit(this)" /></td>
                    <td><asp:Button CssClass="button" ID="button8" runat="server" Text="8" OnClientClick="return addDigit(this)" /></td>
                    <td><asp:Button CssClass="button" ID="button9" runat="server" Text="9" OnClientClick="return addDigit(this)" /></td>
                </tr>
                <tr>
                    <td><asp:ImageButton CssClass="button" ID="backButton" runat="server" ImageUrl="images/arrow_left.png" OnClientClick="return removeDigit()" BorderWidth="1px" /></td>
                    <td><asp:Button CssClass="button" ID="button0" runat="server" Text="0" OnClientClick="return addDigit(this)" /></td>
                    <td><asp:ImageButton CssClass="button" ID="okButton" runat="server" ImageUrl="images/tick.png" OnClick="okButton_Click" BorderWidth="1px" /></td>
                </tr>                                                
            </table>
        </div>
    </form>
</body>
</html>
