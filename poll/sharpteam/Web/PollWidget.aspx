<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PollWidget.aspx.cs" Inherits="PollWidget" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PollWidget</title>
</head>
<body>
    <form id="mainForm" runat="server">
    <div>
        <asp:Label ID="titleLabel" runat="server"></asp:Label>
        <asp:Label ID="errorLabel" runat="server"></asp:Label>
        <asp:RadioButtonList ID="choicesRadioButtonList" runat="server"></asp:RadioButtonList>
        <asp:Button ID="submitButton" runat="server" Text="OK" OnClick="SubmitButtonClick" />
    </div>
    </form>
</body>
</html>
