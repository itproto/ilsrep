<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" MasterPageFile="~/MasterPage.master" %>
<%@ MasterType virtualpath="~/MasterPage.master" %>

<asp:Content ID="leftContentDefault" ContentPlaceHolderID="leftContent" Runat="server">
    <asp:ListView runat="server" ID="pollMenu">
        <LayoutTemplate>
            <ul>
                <li id="itemPlaceholder" runat="server" />
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <a href="Default.aspx?action=showsurvey&id=<%# ((Ilsrep.PollApplication.Communication.Item)Container.DataItem).id %>"><%# ((Ilsrep.PollApplication.Communication.Item)Container.DataItem).name%></a>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>

<asp:Content ID="mainContentDefault" ContentPlaceHolderID="mainContent" Runat="server">
    Main content.
</asp:Content>