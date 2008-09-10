<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<%@page import="ilsrep.poll.common.protocol.Pollsessionlist"%>
<%@page import="ilsrep.poll.common.protocol.Item"%>
<%! 
public String links() throws Exception{
String res="";
res="<ul>";
Pollsessionlist sessions;
DBManager db;
db = new SQLiteDBManager("/pollserver.db");
sessions=db.getPollsessionlist();
 for ( Item sess : sessions.getItems()) {
res+="<li><a href=\"./index.jsp?session="+sess.getId()+"\">"+sess.getName()+"</a></li>\n";
}
res+="<ul>";
return res;
}
%>
