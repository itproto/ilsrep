<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Statistics.aspx.cs" Inherits="Statistics" MasterPageFile="~/MasterPage.master" %>
<%@ MasterType VirtualPath="~/MasterPage.master" %>

<asp:Content ID="leftContentStatistics" ContentPlaceHolderID="leftContent" runat="server">
    <asp:ListView runat="server" ID="surveyMenu">
        <LayoutTemplate>
            <ol>
                <li id="itemPlaceholder" runat="server" />
            </ol>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <a href="Statistics.aspx?object=survey&id=<%#Eval("id")%>"><%#Eval("name")%></a>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>

<asp:Content ID="mainContentStatistics" ContentPlaceHolderID="mainContent" runat="Server">
    <asp:Label ID="message" runat="server" /><br />
    <form id="pagerForm" method="post" runat="server">
        <asp:DataPager ID="statisticsDataPager" runat="server" PagedControlID="statisticsTableList" PageSize="7" OnPreRender="StatisticsPreRender">
            <Fields>      
                <asp:TemplatePagerField>
                    <PagerTemplate>
                        <span>Pages: </span>
                    </PagerTemplate>
                </asp:TemplatePagerField>      
                <asp:numericpagerfield ButtonCount="10" NextPageText="..." PreviousPageText="..." />
            </Fields>
        </asp:DataPager>
    </form>
    <asp:ListView ID="statisticsTableList" runat="server">
        <LayoutTemplate>
            <table cellpadding="0" cellspacing="0" class="statistics_table">
                <tr class="statistics_title">
                    <td colspan="4" runat="server"><b><%=tableTitle%></b></td>
                </tr>
                <tr class="statistics_title">
                    <td>#</td>
                    <td runat="server"><%=objectName%></td>
                    <td>Scores</td>
                    <td>Count of attempts</td>
                </tr>
                <tr id="itemPlaceholder" runat="server"></tr>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td><%=GetIndex()%></td>
                <td><a href="Statistics.aspx?object=user&name=<%#Eval("name")%>"><%#Eval("name")%></a>
                <td><%#Eval("GetScores")%></td>
                <td><%#Eval("attemptsCount")%></td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
    <asp:Image ID="chart" runat="server" />    
</asp:Content>