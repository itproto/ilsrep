<html>
<head> <title>JSP JAVA</title></head>
<body>
<form name="pollsessions" action="poll.jsp" method="get">
<h1>Choose pollsession</h1>
<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<%@page import="ilsrep.poll.common.protocol.Pollsessionlist"%>
<%@page import="ilsrep.poll.common.protocol.Item"%>

<% 
Pollsessionlist sessions;
DBManager db;
db = new SQLiteDBManager("/pollserver.db");
sessions=db.getPollsessionlist();
 for ( Item sess : sessions.getItems()) {
out.println("<input type='radio' name='session' value='"+sess.getId()+"'>"+sess.getName()+"<br>");
}
%>
</br><input type="submit" value="GO">
</form>
</body>
<html>