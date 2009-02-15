<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<%
    final String title = "Poll Application Statistics - Widget";

    final String TYPE_COMMON = "common";

    String[] types = { TYPE_COMMON, "0", "1", "2", "3" };
    String[] values = { "Common statistics",
            "Poll surveys total success/fail statistics",
            "Polls total success/fail statistics",
            "Comparing polls with and without custom choices",
            "Top users by succeed polls" };
%>

<html>

<head>
<meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
<title><%=title%></title>
<meta name="keywords" content="jsp, jteam, poll" />
<meta name="description" content="jteam jsp poll client" />
<link rel="stylesheet" type="text/css" href="../class.css" />
</head>

<body>
<div id="outer">

	<div id="header">
		<h1><span>JSP</span><strong>jteam</strong>Poll</h1>

		<div id="menu">
			<ul>
				<%
				    String reg = "<li class=\"first\"><a href=\"./../index.jsp?register=true\">Register</a></li>";
				    String logout = "<li><a href=\"./../index.jsp?logout=true\">Logout</a></li>";
				    String res = ((session.getAttribute("username") == null) || (request
				            .getParameter("logout") != null)) ? reg : logout;
				    out.println(res);
				%>
				<li><a href="./../index.jsp">Poll Client</a> <font color=white>| </font> 
				<a href="./../editor/index.jsp">Poll Editor</a>
				<font color=white> | Statistics </font>  </li> 
			</ul>
		</div>
	</div>

	<div id="inner">

		<div id="main">
			<div id="xbgA"></div>

			<div id="main_inner">

				<!-- Main start -->
				For statistics widget use next code:<br><br>
<code>&lt;iframe src="http://<%=request.getServerName()%>:<%=request.getServerPort()%><%=request.getContextPath()%>/stats/statsWidget.jsp"/&gt;</code>
				<!-- Main End -->
				<div class="foot"></div>				
			</div>

		</div>

		<div id="side">
			<!-- Side start -->
<ul>
<li class="first"><a href="./statsWidgetInfo.jsp">Statistics Widget</a></li>
<li><a href="./stats.jsp"><%=values[0]%></a></li>
<%
    for (int i = 1; i < types.length; i++) {
%>
<li><a href="./stats.jsp?type=<%=Integer.toString(i - 1)%>"><%=values[i]%></a></li>
<%
    }
%>
</ul>
			<!-- Side end -->
		</div>

		<div  class="foot"></div>
	</div>

</div>
<div id="outer2"></div>

<div id="footer">
	&copy; 2008 InterLogic. Design by Dracony
</div>
  <script type="text/javascript" src="./Nav.js"></script> 
</body>

</html>
