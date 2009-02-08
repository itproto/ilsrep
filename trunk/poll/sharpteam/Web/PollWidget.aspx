<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PollWidget.aspx.cs" Inherits="PollWidget" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PollWidget</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <link href="css/style.css" type="text/css" rel="stylesheet" />    
</head>
<body>
    <div class="widget_title">
        <asp:Label ID="titleLabel" runat="server"></asp:Label>
    </div>
    <asp:Image ID="chart" runat="server" />
    <form id="mainForm" runat="server">
    <div class="widget_poll">
        <div class="widget_error">
            <asp:Label ID="errorLabel" runat="server"></asp:Label>
        </div>
        <asp:RadioButtonList CellSpacing="0" CellPadding="0" ID="choicesRadioButtonList" runat="server"></asp:RadioButtonList>
        <div class="widget_button_container">
            <asp:ImageButton CssClass="widget_submit_button" ID="submitButton" runat="server" OnClick="SubmitButtonClick" ImageUrl="images/submit.gif" />
        </div>
    </div>
    </form>    
</body>
</html>
