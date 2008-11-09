<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
</head>
<body>
    <p class="title">Login</p> 
    <span id="status" class="text" runat="Server"/>
    <form id="Form1" runat="server">
    Username: <asp:textbox id="username" cssclass="text" runat="Server"/><br />
    Password: <asp:textbox id="password" textmode="Password" cssclass="text" runat="Server"/><br />
    <asp:Button ID="loginButton" OnClick="Login_Click" Text="  Login  " CssClass="button" runat="server" />
    <asp:Button ID="backButton" OnClick="Back_Click" Text="  Return  " CssClass="button" runat="server" Visible="false" />
    </form>
</body>
</html>
