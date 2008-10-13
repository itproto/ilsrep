<%@ include file="./poll.jsp" %>
<%@ include file="./links.jsp" %>
<%@page import="javax.servlet.ServletRequest"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
<head>
<meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
<title>JSP POLL</title>
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
				<li class="first"><a href="./index.jsp?register=true">Register</a></li>
				 <li><a href="./index.jsp?logout=true">Logout</a></li>
				<!--<li><a href="#">here</a></li> -->
			</ul>
		</div>
	</div>

	<div id="inner">

		<div id="main">
			<div id="xbgA"></div>
	
			<div id="main_inner">

				<!-- Main start -->
<%
if(request.getParameter("session")!=null){
	out.println(editPoll(request.getParameter("session")));
	} else {
	out.println(links()); 	
		}
%>			

				<!-- Main End -->
				<div class="foot"></div>				
			</div>
	
		</div>
	
		<div id="side">
			<!-- Side start -->

<% out.println(links()); 
%>
				
			<!-- Side end -->
		</div>
		<div id="xbgB" class="foot"></div>
	</div>

</div>
<div id="outer2"></div>

<div id="footer">
	&copy; 2008 InterLogic. Design by Dracony
</div>
    <script type="text/javascript" src="./xmlw3cdom.js"></script>
    <script type="text/javascript" src="./xmlsax.js"></script>
   <script type="text/javascript" src="./formfunctions.js"></script> 
</body>
</html>