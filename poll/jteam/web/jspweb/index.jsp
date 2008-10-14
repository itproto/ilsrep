<%@ include file="./poll.jsp" %>
<%@ include file="./submit.jsp" %>
<%@ include file="./register.jsp" %>
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
				<li class="first"><a href="./index.jsp?register=true">Register</a></li>
				 <li><a href="./index.jsp?logout=true">Logout</a></li>
				<li><a href="./editor/index.jsp">Poll Editor</a></li>
			</ul>
		</div>
	</div>

	<div id="inner">

		<div id="main">
			<div id="xbgA"></div>
	
			<div id="main_inner">

				<!-- Main start -->
	
				<%
				if ((request.getParameter("logout")!=null)) {
				session.removeAttribute("username");
				out.println(login(session,request.getParameter("name"),request.getParameter("password")));
					} else {
				if(request.getParameter("register")==null){
if((request.getParameter("session")!=null) && (request.getParameter("poll")!=null ) && (session.getAttribute("username")!=null)) {
out.println(getPoll(request.getParameter("session"),request.getParameter("poll"),  session, request.getParameter("choice"),request.getParameter("custom")));
} else {

out.println(login(session,request.getParameter("name"),request.getParameter("password")));
}
} else {
	out.println(register(request.getParameter("name"),request.getParameter("password"),request.getParameter("password2")));
	}
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