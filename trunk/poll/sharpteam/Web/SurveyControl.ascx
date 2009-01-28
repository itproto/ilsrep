<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SurveyControl.ascx.cs" Inherits="SurveyControl" %>

<h1><asp:Label ID="surveyName" runat="server" /></h1>

<h3><asp:Label ID="pollName" runat="server" /></h3>

<asp:Label ID="pollDesc" runat="server" />

<form action="Survey.aspx" method="post" runat="server">
<asp:Label ID="errorMessage" CssClass="error" runat="server" />
<asp:RadioButtonList runat="server" ID="choiceList"></asp:RadioButtonList>

<br />

<asp:Button ID="btnSurvey" Text="Next" runat="server" onclick="btnSurvey_Click" CssClass="alignRight" />
</form>