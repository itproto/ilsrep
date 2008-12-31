﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Results.aspx.cs" Inherits="Results" %>

<asp:Content ID="mainContentResults" ContentPlaceHolderID="mainContent" Runat="Server">
    <h3>Here is your choices:</h3>
    
    <asp:ListView runat="server" ID="surveyResults">
        <LayoutTemplate>
            <ol>
                <li id="itemPlaceholder" runat="server" />
            </ol>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <%# Eval("pollName") %>: <%# Eval("choice") %>
            </li>
        </ItemTemplate>
    </asp:ListView>
    
    <h3>Here is your Survey results:</h3>

    Your score: <asp:Label ID="lblScore" runat="server" />
    
    <h3><asp:Label ID="lblResult" runat="server" /></h3>
</asp:Content>

