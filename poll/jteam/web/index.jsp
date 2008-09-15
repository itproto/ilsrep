<%@ include file="./links.jsp" %>
<%@ include file="./poll.jsp" %>
<%@ include file="./submit.jsp" %>
<%@page import="javax.servlet.ServletRequest"%>
<html>
<head>
<link rel="stylesheet" type="text/css" href="class.css" />
</head>
<body>
<div id="main">
<div id="head">POLL<img src="./logo.png">CLIENT
</div>
<div id="sidebar">
<%
out.println(links());
 %>
</div>
<div id="point">
<%
if(request.getParameter("sub")!=null){
out.println(getRes(request));} else if(request.getParameter("session")!=null) {
out.println(getPoll(request.getParameter("session")));
} else {

out.println(PLEASE_ENTER_POLL);
}
 %>
</div>
</div>
</body>
<html>