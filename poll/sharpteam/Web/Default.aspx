<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PollClientASP</title>
    <link href="StyleSheet.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div class="error"><%= errorMessage %></div>
    <form action="?do=login" method="post">
    <table align="center">
    <tr><td>Username:</td><td><input type="text" name="username" /></td></tr>
    <tr><td>Password:</td><td><input type="password" name="password" /></td></tr>
    <tr><td colspan="2" align="center"><input type="submit" value="Login" /> <input type="button" value="Register" onclick="document.location='Register.aspx'" /></td></tr>
    </table>
    </form>
</body>
</html>
