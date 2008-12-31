<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" MasterPageFile="~/MasterPage.master" %>
<%@ MasterType virtualpath="~/MasterPage.master" %>

<asp:Content ID="leftContentDefault" ContentPlaceHolderID="leftContent" Runat="server">
    <asp:ListView runat="server" ID="surveyMenu">
        <LayoutTemplate>
            <ol>
                <li id="itemPlaceholder" runat="server" />
            </ol>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <a href="Survey.aspx?id=<%# Eval("id") %>"><%# Eval("name") %></a>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>

<asp:Content ID="mainContentDefault" ContentPlaceHolderID="mainContent" Runat="server">
    Please select survey from left menu.
</asp:Content>