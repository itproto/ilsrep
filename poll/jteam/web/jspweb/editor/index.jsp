<%@page import="webservice.endpoint.WebJPoll_Service"%>
<%@page import="javax.xml.ws.WebServiceRef"%>
<%@page import="javax.xml.namespace.QName"%>
<%@page import="java.net.URL"%>
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
<%@ include file="./save.jsp" %>
<%@ include file="./widget.jsp" %>
<%@ include file="./saveWidget.jsp" %>
<%@page import="javax.servlet.ServletRequest"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
<head>
<meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
<title>JSP POLL</title>
<meta name="keywords" content="jsp, jteam, poll" />
<meta name="description" content="jteam jsp poll client" />
<link rel="stylesheet" type="text/css" href="../class.css" />
<script type='text/javascript'>
var start=0;
function allow(){
	start++;
	if ((start==7)&&(document.getElementById("frmEdit")!=null)) {
		$.fn.page.defaults.func = navigateUserList; 
		$.fn.page.defaults.subject = '#main_inner'; 	
$('#cmdMoveFirst').click(function(){$(this).page({direction : 'first'})});
$('#cmdMovePrevious').click(function(){$(this).page({direction : 'previous'})});
$('#cmdMoveNext').click(function(){$(this).page({direction : 'next'})});
$('#cmdMoveLast').click(function(){$(this).page({direction : 'last'})});
		startedit();
			}
	}

</script>
  <script type="text/javascript" src="./xmlw3cdom.js" onload="allow();"></script>
    <script type="text/javascript" src="./xmlsax.js" onload="allow();" ></script>
   <script type="text/javascript" src="./formfunctions.js" onload="allow();"></script> 
   <script type="text/javascript" src="./xmlEscape.js" onload="allow();"></script> 
    <script type="text/javascript" src="./jquery.js" onload="allow();"></script> 
    <script type="text/javascript" src="./jquery.page.js" onload="allow();"></script> 
      <!--[if IE]><script defer src="ie_onload.js"></script><![endif]-->

</head>
<body onload="allow();">

<div id="outer">

	<div id="header">
		<h1><span>JSP</span><strong>jteam</strong>Poll</h1>
		<div id="menu">
			<ul>
				<%
				
			String reg="<li class=\"first\"><a href=\"./../index.jsp?register=true\">Register</a></li>";
				 String logout="<li><a href=\"./../index.jsp?logout=true\">Logout</a></li>";
				 String res=((session.getAttribute("username")==null)||(request.getParameter("logout")!=null)) ? reg : logout;
				 out.println(res);
				 %>
				<li><a href="./../index.jsp">Poll Client</a> <font color=white> | Poll Editor | </font>
				<a href="./../stats/stats.jsp">Statistics</a></li>
			</ul>
		</div>
	</div>

	<div id="inner">

		<div id="main">
			<div id="xbgA"></div>
	
			<div id="main_inner">

				<!-- Main start -->
<%
if(request.getParameter("result")==null){
if(request.getParameter("session")!=null){
	out.println(editPoll(request.getParameter("session"),(String)session.getAttribute("username")));
	} else {
		if(request.getParameter("widget")!=null){
			out.println(newWidget("new",(String)session.getAttribute("username")));
			} else {
	out.println(links(false)); 
}
		}
		} else {
		if(request.getParameter("saveWidget")!=null){
		out.println(saveWidgetToDB(request.getParameter("sessiontype"),request.getParameter("result"),request.getServerName()+":"+Integer.toString(request.getServerPort())));
			} else {
			
			out.println(saveToDB(request.getParameter("sessiontype"),request.getParameter("result")));
		}
			}
%>			

				<!-- Main End -->
								
			</div>
	
		</div>
	
		<div id="side">
			<!-- Side start -->

<% out.println(links(true)); 

%>
				
			<!-- Side end -->
		</div> 
		<div   class="foot"></div>
	</div>

</div>
<div id="outer2"></div>

<div id="footer">
	&copy; 2008 InterLogic. Design by Dracony
</div>
  
 
</body>
</html>