<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="login.aspx.cs" Inherits="login" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head>
    <title>Login</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <link href="css/style.css" type="text/css" rel="Stylesheet" />
    <script src="js/scripts.js" type="text/javascript"></script>
    <style>      
        .login_body
        {
	        background-color: #CCCC99;
	        font-family: Verdana;
	        font-size: 10px;
	        text-align: center;
	        margin: 0;
	        padding: 0;
        }
              
        .login_main
        {
        	background-color: #FFFFE6;
        	color: #804000;
        	width: 215px;
        	border: 2px #BD2121 solid;
        	text-align: right;
        	margin: 100px auto;
        	padding: 5px;
        }
        
        .login_title
        {
        	text-align: center;
        	font-weight: bold;
        	font-size: 11px;
        	margin: 0px;
        	padding: 0px;
        }           
        
        .login_field
        {
        	margin-top: 3px;
        	padding: 0px;
        }
        
        .login_message
        {
        	color: Red;
        	margin-top: 3px;
        }
        
        .login_buttons
        {
        	margin-top: 3px;
        }
    </style>
</head>
<body class="login_body">
    <div class="login_main">    
        <form id="loginForm" runat="server">
            <div class="login_title">Login</div>
            <div class="login_field">Username: <asp:textbox id="username" runat="Server"/></div>
            <div class="login_field">Password: <asp:textbox id="password" textmode="Password" runat="Server"/></div>
            <div class="login_message"><span id="message" runat="Server" /></div>
            <div class="login_buttons">
                <asp:Button ID="loginButton" OnClick="Login_Click" Text="  Login  " runat="server" />
                <asp:Button ID="backButton" OnClick="Back_Click" Text="  Return  " runat="server" Visible="false" />
            </div>
        </form>
    </div>
</body>
</html>
