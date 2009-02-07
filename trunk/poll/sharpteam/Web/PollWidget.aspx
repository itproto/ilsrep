<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PollWidget.aspx.cs" Inherits="PollWidget" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PollWidget</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <link href="css/style.css" type="text/css" rel="stylesheet" />    
</head>
<body>
    <asp:Image ID="chart" runat="server" />
    <form id="mainForm" runat="server">
    <div>
        <div>
            <asp:Label ID="titleLabel" runat="server"></asp:Label>
        </div>
        <div class="widget_error">
            <asp:Label ID="errorLabel" runat="server"></asp:Label>
        </div>
        <asp:RadioButtonList ID="choicesRadioButtonList" runat="server"></asp:RadioButtonList>
        <asp:Button ID="submitButton" runat="server" Text="OK" OnClick="SubmitButtonClick" />
    </div>
    </form>    
</body>
</html>
