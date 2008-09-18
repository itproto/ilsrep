<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PollClientASP</title>
</head>
<body>
    <ul>
        <%
            if (pollSessionsList != null)
            {
                foreach (Ilsrep.PollApplication.Communication.Item item in pollSessionsList)
                {
                    %>
                        <li><%=item.name%></li>
                    <%
                }
            }
        %>
    </ul>
</body>
</html>
