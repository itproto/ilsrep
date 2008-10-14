<%@page import="ilsrep.poll.common.model.Poll"%>
<%@page import="ilsrep.poll.common.model.Choice"%>
<%@page import="ilsrep.poll.common.model.Pollsession"%>
<%@page import="ilsrep.poll.server.db.SQLiteDBManager"%>
<%@page import="ilsrep.poll.server.db.DBManager"%>
<%@page import="ilsrep.poll.common.protocol.Pollsessionlist"%>
<%@page import="ilsrep.poll.common.protocol.Item"%>
<%! 
public String links() throws Exception{
String links="";
links="<ul>";
Pollsessionlist sessions;
DBManager db;try {
db = new SQLiteDBManager(null,getServletContext().getRealPath("/")+"/pollserver.db");
sessions=db.getPollsessionlist();
 for ( Item sess : sessions.getItems()) {
links+="<li><a href=\"./index.jsp?session="+sess.getId()+"&poll=0 \">"+sess.getName()+"</a></li>\n";
}
links+="<li><a href=\"./index.jsp?session=new\">Create New Session</a></li>";
links+="</ul>";} catch (Exception e){links=e.getMessage();}
return links;
}
%>
