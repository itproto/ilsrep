<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Statistics.aspx.cs" Inherits="Statistics" %>

<asp:Content ID="mainContentStatistics" ContentPlaceHolderID="mainContent" Runat="Server">
    <asp:Label ID="message" runat="server" /><br />
    <asp:Label ID="pagesLabel" runat="server" />
    <asp:Table ID="statisticsTable" CellPadding="0" CellSpacing="0" runat="server" CssClass="statistics_table"></asp:Table>
    <asp:Image ID="chart" runat="server" />
</asp:Content>

<asp:Content ID="leftContentStatistics" ContentPlaceHolderID="leftContent" Runat="server">
    <asp:ListView runat="server" ID="surveyMenu">
        <LayoutTemplate>
            <ol>
                <li id="itemPlaceholder" runat="server" />
            </ol>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <a href="Statistics.aspx?object=survey&id=<%# Eval("id") %>"><%# Eval("name") %></a>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>