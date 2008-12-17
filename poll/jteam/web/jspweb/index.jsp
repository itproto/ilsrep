<%@page import="webservice.endpoint.WebJPoll_Service"%>
<%@page import="javax.xml.ws.WebServiceRef"%>
<%@page import="javax.xml.namespace.QName"%>
<%@page import="java.net.URL"%>
<%!
WebJPoll_Service service;
public void setService(String name,String port) { try{
		QName serviceName = new QName("http://endpoint.webservice/","WebJPoll");
URL url = new URL("http://"+name+":"+port+"/WebJPoll/WebJPoll?wsdl"); 
service=new WebJPoll_Service(url,serviceName);
} catch(Exception e){};
}
%>
<%
setService(request.getServerName(),Integer.toString(request.getServerPort()));
%>
<%@ include file="./poll.jsp" %>
<%@ include file="./submit.jsp" %>
<%@ include file="./register.jsp" %>
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
					<%
					String output="";
						if ((request.getParameter("logout")!=null)) {
				session.removeAttribute("username");
				output=(login(session,request.getParameter("name"),request.getParameter("password")));
					} else {
				if(request.getParameter("register")==null){
if((request.getParameter("session")!=null) && (request.getParameter("poll")!=null ) && (session.getAttribute("username")!=null)) {
output=(getPoll(request.getParameter("session"),request.getParameter("poll"),  session, request.getParameter("choice"),request.getParameter("custom")));
} else {

output=(login(session,request.getParameter("name"),request.getParameter("password")));
}
} else {
	output=(register(request.getParameter("name"),request.getParameter("password"),request.getParameter("password2")));
	}
}

			String reg="<li class=\"first\"><a href=\"./index.jsp?register=true\">Register</a></li>";
				 String logout="<li><a href=\"./index.jsp?logout=true\">Logout</a></li>";
				 String res=((session.getAttribute("username")==null)||(request.getParameter("logout")!=null)) ? reg : logout;
				 out.println(res);
				 %>
				<li> <font color=white> Poll Client | </font>
				<a href="./editor/index.jsp">Poll Editor</a></li>
			</ul>
		</div>
	</div>

	<div id="inner">

		<div id="main">
			<div id="xbgA"></div>
	
			<div id="main_inner">

				<!-- Main start -->
	
				<%
		out.println(output);
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