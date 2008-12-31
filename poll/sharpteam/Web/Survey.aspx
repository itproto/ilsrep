<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Survey.aspx.cs" Inherits="_Survey" %>
<%@ Register Src="~/SurveyControl.ascx" TagPrefix="uc" TagName="Survey" %>

<asp:Content ID="mainContentSurvey" ContentPlaceHolderID="mainContent" Runat="Server">
    <uc:Survey ID="surveyControl" runat="server" />
</asp:Content>

<asp:Content ID="leftContentSurvey" ContentPlaceHolderID="leftContent" Runat="Server">
    <asp:ListView runat="server" ID="pollMenu">
        <LayoutTemplate>
            <ol>
                <li id="itemPlaceholder" runat="server" />
            </ol>
        </LayoutTemplate>
        <ItemTemplate>
            <li class="<%# Convert.ToInt32(Eval("Id")) == surveyControl.currentPollID ? "selected" : "" %>">
                <%# Eval("Name") %>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>