<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Widget.aspx.cs" Inherits="Widget" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPage.master" %>

<asp:Content ID="widgetMainContent" ContentPlaceHolderID="mainContent" Runat="Server">
    <asp:Label ID="errorLabel" runat="server"></asp:Label>
    
    <asp:ListView ID="poll" runat="server" OnItemCommand="OnItemCommandOccur">
        <LayoutTemplate>
            <form id="pollForm" runat="server">
                <table cellpadding="0" cellspacing="0" class="widget_table">
                    <tr>
                        <td class="widget_header">Question</td>
                        <td colspan="2">
                            <asp:TextBox ID="questionTextBox" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="itemPlaceholder" runat="server"></tr>
                    <tr>
                        <td class="widget_header">New answer</td>
                        <td>
                            <asp:TextBox ID="newAnswerTextBox" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="addButton" ImageUrl="images/add.gif" CssClass="widget_button" CommandName="AddItem" runat="server" Text="Add" />
                        </td>
                    </tr>
                </table>
            </form>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td class="widget_header">Answer<%=GenerateAnswerID()%></td>
                <td>
                    <asp:TextBox ID="answerTextBox" runat="server" Text='<%#Eval("choice")%>'></asp:TextBox>
                </td>
                <td>
                    <asp:ImageButton ID="deleteButton" ImageUrl="images/delete.gif" CssClass="widget_button" CommandName="RemoveItem" Visible="<%#enableButtons%>" CommandArgument='<%#Eval("Id")%>' runat="server" />
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>

