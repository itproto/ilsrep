<%@ include file="./links.jsp" %>
<%@ include file="./poll.jsp" %>
<%@ include file="./submit.jsp" %>
<%@page import="javax.servlet.ServletRequest"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
<head>
<meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
<title>JSP POLL</title>
<meta name="keywords" content="jsp, jteam, poll" />
<meta name="description" content="jteam jsp poll client" />
<link rel="stylesheet" type="text/css" href="class.css" />
</head>
<body>

<div id="outer">

	<div id="header">
		<h1><span>JSP</span><strong>jteam</strong>Poll</h1>
		<div id="menu">
			<ul>
				<li class="first"><a href="#">some</a></li>
				<li><a href="#">links</a></li>
				<li><a href="#">here</a></li>
			</ul>
		</div>
	</div>

	<div id="inner">

		<div id="main">
			<div id="xbgA"></div>
	
			<div id="main_inner">

				<!-- Main start -->
	
				<%
if(request.getParameter("sub")!=null){
out.println(getRes(request));} else if(request.getParameter("session")!=null) {
out.println(getPoll(request.getParameter("session")));
} else {

out.println(PLEASE_ENTER_POLL);
}
 %>				
				
				<!-- Main End -->
				<div class="foot"></div>				
			</div>
	
		</div>
	
		<div id="side">
			<!-- Side start -->

<% out.println(links()); %>
				
			<!-- Side end -->
		</div>
		<div id="xbgB" class="foot"></div>
	</div>

</div>
<div id="outer2"></div>

<div id="footer">
	&copy; 2008 InterLogic. Design by Dracony
</div>

</body>
</html>