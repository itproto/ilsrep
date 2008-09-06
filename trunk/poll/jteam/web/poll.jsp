<html>
<head> <title>JSP JAVA</title></head>
<body>
<form name="polls" action="submit.jsp" method="get">
<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<% 
out.println("<input type='hidden' name='session' value='"+request.getParameter("session")+"' >");
Pollsession sess;
DBManager db;
db = new SQLiteDBManager("/pollserver.db");
sess=db.getPollsessionById(request.getParameter("session"));
out.println("<h1>"+sess.getName()+"</h1>");
 for (Poll cur : sess.getPolls()) {
out.println("<h2>"+cur.getName()+"</h2>");
out.println("<h3>"+cur.getDescription().getValue()+"</h3>");
for( Choice ch : cur.getChoices()){
out.println("<input type='radio' name='"+cur.getName()+"' value='"+ch.getName()+"'>"+ch.getName()+"<br>");
}
if(cur.getCustomEnabled().equals("true")){
out.println("<input type='radio' name='"+cur.getName()+"' value='custom_choice'>Custom<input type='textarea' name='custom"+cur.getName()+"'>");
}
}
%>
</br><input type="submit" value="GO">
</form>
</body>
<html>