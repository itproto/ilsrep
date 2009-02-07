<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Widget.aspx.cs" Inherits="Widget" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPage.master" %>

<asp:Content ID="widgetMainContent" ContentPlaceHolderID="mainContent" Runat="Server">
    <div class="widget_error">
        <asp:Label ID="errorLabel" runat="server"></asp:Label>
    </div>
    
    <asp:ListView ID="poll" runat="server" OnItemCommand="OnItemCommandOccur">
        <LayoutTemplate>
            <form id="pollForm" runat="server">
                <table cellpadding="0" cellspacing="0" class="widget_table">
                    <tr>
                        <td>Question</td>
                        <td>
                            <asp:TextBox ID="questionTextBox" runat="server"></asp:TextBox>
                        </td>
                        <td></td>
                    </tr>
                    <tr id="itemPlaceholder" runat="server"></tr>
                    <tr>
                        <td>New answer</td>
                        <td>
                            <asp:TextBox ID="newAnswerTextBox" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="addButton" ImageUrl="images/add.gif" CssClass="widget_button" CommandName="AddItem" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td class="widget_header">
                            <asp:ImageButton CssClass="widget_submit_button" ID="submitButton" runat="server" CommandName="SubmitWidget" ImageUrl="images/submit.gif" />
                        </td>
                        <td></td>
                    </tr>
                </table>
            </form>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td>Answer<%=GenerateAnswerID()%></td>
                <td>
                    <asp:TextBox ID="answerTextBox" runat="server" Text='<%#Eval("choice")%>'></asp:TextBox>
                </td>
                <td>
                    <asp:ImageButton ID="deleteButton" ImageUrl="images/delete.gif" CssClass="widget_button" CommandName="RemoveItem" Visible="<%#enableButtons%>" CommandArgument='<%#Eval("Id")%>' runat="server" />
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>

    <iframe style="border: solid 1px #A30313; width: 400px; height: 200px" scrolling="no" src="PollWidget.aspx?poll_id=3"></iframe>  
</asp:Content>

